using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Channels;

using Radiance;
using Radiance.Markup;
using Lionsguard;

namespace Perenthia
{
	public static class MapManager
	{
		public delegate void PlayerZoneDownloadCompleteCallback();
		public delegate void PlayerZoneChunkCompleteCallback(int current, int total);
		public delegate void MapLoadedCallback(int startX, int startY, List<RdlPlace> places);

		public const int MapWidth = 15;
		public const int MapHeight = 15;

		public static void BeginMapDownload(string playerZone, bool includeActors,
			PlayerZoneDownloadCompleteCallback playerZoneDownloadCompleteCallback,
			PlayerZoneChunkCompleteCallback playerZoneChunkCompleteCallback,
			MapLoadedCallback mapLoadedCallback)
		{
			MapState state = new MapState
			{
				PlayerZone = playerZone,
				IncludeActors = includeActors,
				PlayerZoneCallback = playerZoneDownloadCompleteCallback,
				PlayerChunkCallback = playerZoneChunkCompleteCallback,
				MapLoadedCallback = mapLoadedCallback
			};

			var client = ServiceManager.CreateDepotCommunicator();
			client.SendCommand(new RdlCommand("MAPNAMES"), (e) =>
				{
					if (e.Tags.Count > 0)
					{
						// Handle map names.
						Game.LoadMapDetails(e.Tags.GetTags<RdlTag>("MAP", "MAP"));

						// Display points on the map where the map details are defined.
						QueueMaps(state);
					}
					client.Close();
				});
		}

		private static void QueueMaps(MapState state)
		{
			state.MapDetails = new Queue<MapDetail>();
			var curMap = Game.Maps.Where(m => String.Compare(m.Name, state.PlayerZone, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
			if (curMap != null)
			{
				// Make the player zone the first map to be loaded.
				state.MapDetails.Enqueue(curMap);
			}

			foreach (var map in Game.Maps)
			{
				if (curMap != null && String.Compare(map.Name, curMap.Name, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					state.MapDetails.Enqueue(map);
				}
			}
			DownloadMaps(state);
		}

		private static void DownloadMaps(MapState state)
		{
			if (state.MapDetails.Count > 0)
			{
				MapDetail detail = state.MapDetails.Dequeue();
				if (StorageManager.RequiresFileUpdate(detail.Name))
				{
					// Chunk up the map and download the chunks.
					state.CurrentMap = detail;
					state.CurrentChunk = null;
					state.Places = new List<RdlPlace>();
					ChunkMap(state);
				}
				else
				{
					if (state.MapLoadedCallback != null)
					{
						state.MapLoadedCallback(detail.StartX, detail.StartY,
							StorageManager.ReadMap(detail.Name).GetPlaces());
					}
					if (state.PlayerZoneCallback != null && (detail.Name == state.PlayerZone))
					{
						state.PlayerZoneCallback();
					}
					DownloadMaps(state);
				}
			}
		}

		private static void ChunkMap(MapState state)
		{
			if (state.CurrentChunk == null || state.CurrentChunk.MapName != state.CurrentMap.Name)
			{
				Logger.LogDebug("MapName = {0}", state.CurrentMap.Name);
				state.CurrentChunk = new MapChunkInternal
				{
					MapName = state.CurrentMap.Name,
					StartX = state.CurrentMap.StartX,
					StartY = state.CurrentMap.StartY,
					EndX = state.CurrentMap.StartX + state.CurrentMap.Width,
					EndY = state.CurrentMap.StartY + state.CurrentMap.Height,
					Width = state.CurrentMap.Width,
					Height = state.CurrentMap.Height
				};
			}
			else
			{
				state.CurrentChunk.MoveNext();
			}

			var client = ServiceManager.CreateDepotCommunicator();
			client.SendCommand(new RdlCommand("MAPCHUNK", state.CurrentMap.Name, state.CurrentChunk.StartX, state.CurrentChunk.StartY, state.IncludeActors),
				(e) =>
				{
					if (e.Tags.Count > 0)
					{
						if (state.CurrentMap.Name == state.PlayerZone)
						{
							if (state.PlayerChunkCallback != null)
							{
								int total = (state.CurrentMap.Width > state.CurrentMap.Height) ? state.CurrentMap.Width / MapWidth : state.CurrentMap.Height / MapHeight;
								int current = (state.CurrentMap.Width > state.CurrentMap.Height) ? (MapWidth - state.CurrentMap.StartX) / MapWidth : (MapHeight - state.CurrentMap.StartY) / MapHeight;

								state.PlayerChunkCallback(current, total);
							}
						}

						List<RdlPlace> places = e.Tags.GetPlaces();
						state.Places.AddRange(places);

						if (state.CurrentChunk.IsComplete)
						{
							// Map download is complete.
							if (state.CurrentMap.Name == state.PlayerZone && state.PlayerZoneCallback != null)
							{
								state.PlayerZoneCallback();
							}

							// Save the map to disk.
							Logger.LogDebug("Saving map to disk: MapName={0}, PlaceCount={1}",
								state.CurrentMap.Name, state.Places.Count);
							StorageManager.WriteMap(state.CurrentMap.Name, state.Places.ToTagCollection());

							if (state.MapLoadedCallback != null)
								state.MapLoadedCallback(state.CurrentMap.StartX, state.CurrentMap.StartY, state.Places);

							// Download the next map.
							DownloadMaps(state);
						}
						else
						{
							// Map not completely downloaded, store the tags and re-chunk the next piece of the map.
							ChunkMap(state);
						}
					}
					client.Close();
				});
		}

		private class MapState
		{
			public string PlayerZone { get; set; }
			public bool IncludeActors { get; set; }
			public PlayerZoneDownloadCompleteCallback PlayerZoneCallback { get; set; }
			public PlayerZoneChunkCompleteCallback PlayerChunkCallback { get; set; }
			public MapLoadedCallback MapLoadedCallback { get; set; }
			public MapDetail CurrentMap { get; set; }
			public Queue<MapDetail> MapDetails { get; set; }
			public MapChunkInternal CurrentChunk { get; set; }
			public List<RdlPlace> Places { get; set; }
		}

		private class MapChunkInternal
		{
			public string MapName { get; set; }
			public int StartX { get; set; }
			public int StartY { get; set; }
			public int EndX { get; set; }
			public int EndY { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }

			public bool IsComplete
			{
				get { return this.StartX >= this.EndX && this.StartY >= this.EndY; }
			}

			private int _maxX = 0;
			private int _maxY = 0;
			private int _x = 0;
			private int _y = 0;
			public void MoveNext()
			{
				if (_maxX == 0) _maxX = (this.EndX - this.StartX) / MapWidth;
				if (_maxY == 0) _maxY = (this.EndY - this.StartY) / MapHeight;

				Logger.LogDebug("_x = {0}, _y = {1}, _maxX = {2}, _maxY = {3}", _x, _y, _maxX, _maxY);
				if (_y < _maxY)
				{
					if (_x < _maxX)
					{
						this.StartX += MapWidth;
						_x++;
					}
					else
					{
						this.StartX = this.EndX - this.Width;
						this.StartY += MapHeight;
						_x = 0;
						_y++;
					}
				}
				else
				{
					this.StartX = this.EndX;
					this.StartY = this.EndY;
				}
				Logger.LogDebug("StartX = {0}, StartY = {1}, EndX = {2}, EndY = {3}",
					this.StartX, this.StartY, this.EndX, this.EndY);
			}
		}
	}
}
