using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;

using Lionsguard;

using Radiance;
using Radiance.Markup;

using Perenthia;

namespace Perenthia.Controls
{
	public partial class MapEditor : UserControl
	{
		// EDITOR TODO:
		// TODO: "Define Maps" button to draw map regions.
		// TODO: "Show All Levels" button to show all z-index levels, all faded except current. (Default when "Define Maps" clicked)

		private List<RdlTerrain> _terrain = new List<RdlTerrain>();

		private Terrain _selectedTerrain = new Terrain();
		private string _selectedPlaceName = String.Empty;

		private bool _isDragging = false;
		private bool _isZoomIn = false;
		private bool _isZoomOut = false;

		private Point _originalMousePosition = new Point();
		private Point _mousePosition = new Point();
        private Point3 _centerPosition = new Point3();
		private RectangleGeometry _clip;

		private MapAction _action = MapAction.Select;

		private UIElement _dragCursor = null;

		private int _zoomEditValue = 22;
		private int _zIndex = 0;

		private bool _showMaps = false;

		private Tile _selectedTile = null;

		private List<Tile> _tiles = new List<Tile>();

		//private Dictionary<MapKey, List<RdlPlace>> _maps = new Dictionary<MapKey, List<RdlPlace>>();

		private List<RdlPlace> _places = new List<RdlPlace>();

		private enum ZoomLevel
		{
			Overworld,
			Region,
			Map,
		}

		private enum MapAction
		{
			Select,
			Draw,
			Fill,
			Exits,
		}

		public delegate void StatusUpdatedEventHandler(string status);

		public event StatusUpdatedEventHandler StatusUpdated = delegate { };

		public event PlaceEventHanlder PlaceChanged = delegate { };

		public event EventHandler MapLoadComplete = delegate { };

		public MapEditor()
		{
			this.Loaded += new RoutedEventHandler(MapEditor_Loaded);
			InitializeComponent();
		}

		private void MapEditor_Loaded(object sender, RoutedEventArgs e)
		{
			_clip = new RectangleGeometry { Rect = new Rect(0, 0, this.MapContainer.Width, this.MapContainer.Height) };
			ctlViewPort.Clip = _clip;

			ctlNav.DirectionClick += new DirectionEventHandler(ctlNav_DirectionClick);
		}

		public void Initialize()
		{
			this.SetAction(MapAction.Select);
			this.LoadTerrain();
		}

        public void UpdateActor(RdlActor actor)
        {
            var tile = _tiles.Where(t => t.Place.Actors.Count(a => a.ID == actor.ID) > 0).FirstOrDefault();
            if (tile != null)
            {
                int index = -1;
                for (int i = 0; i < tile.Place.Actors.Count; i++)
                {
                    if (tile.Place.Actors[i].ID == actor.ID)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0)
                {
                    tile.Place.Actors[index] = actor;
                }
            }
        }

		private void ctlNav_DirectionClick(DirectionEventArgs e)
		{
			if (_selectedTile != null)
			{
				Direction direction = Direction.FromName(e.Direction);

				// Attempt to create an exit in the specified direction.
				Point3 location = _centerPosition + direction.Value;

				var dest = _tiles.Where(t => t.Location == location).FirstOrDefault();
				if (dest != null && dest.Place != null)
				{
					// Add an exit to the selected tile in the current direction.
					_selectedTile.Place.Properties.SetValue(String.Concat("Exit_", direction.Name), true);
					_selectedTile.IsModified = true;
					_selectedTile.RefreshExits();

					// Add an exit to the dest tile in the counter direction.
					dest.Place.Properties.SetValue(String.Concat("Exit_", direction.CounterDirection.Name), true);
					dest.IsModified = true;
					dest.RefreshExits();

					this.SetSelectedTile(dest);

					// Move the map in the specified direction.
					this.SetView(location);
				}
				else
				{
					MessageBox.Show("A place does not exist in that direction.", "ERROR", MessageBoxButton.OK);
				}
			}
			else
			{
				MessageBox.Show("You must select a tile in order to create exits.", "ERROR", MessageBoxButton.OK);
			}
		}

		#region Load Map Components
		private void LoadTerrain()
		{
            cboTerrain.ItemsSource = Game.Terrain;

			//BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
			//client.GetPlaceTypesCompleted += new EventHandler<GetPlaceTypesCompletedEventArgs>(client_GetPlaceTypesCompleted);
			//client.GetPlaceTypesAsync(_token);
		}

        private void client_GetPlaceTypesCompleted(object sender, GetPlaceTypesCompletedEventArgs e)
        {
            if (e.Result.Success)
            {
                var placeTypes = RdlTagCollection.FromString(e.Result.TagString).Where(t => t.TagName == "PLACETYPE" && t.TypeName == "PLACETYPE");
                if (placeTypes.Count() > 0)
                {
					List<PlaceTypeDesc> types = new List<PlaceTypeDesc>();
                    foreach (var pt in placeTypes)
                    {
						types.Add(new PlaceTypeDesc { Name = pt.GetArg<string>(0) });
                    }
                    this.Dispatcher.BeginInvoke(() => this.BindPlaceTypes(types));
                }

				//BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				//client.GetMapNamesCompleted += new EventHandler<GetMapNamesCompletedEventArgs>(client_GetMapNamesCompleted);
				//client.GetMapNamesAsync(_token);
            }
        }

		private void BindPlaceTypes(IEnumerable<PlaceTypeDesc> types)
        {
            cboPlaceType.ItemsSource = types;
        }

		private void client_GetMapNamesCompleted(object sender, GetMapNamesCompletedEventArgs e)
		{
			if (e.Result.Success)
			{
				// Handle map names.
				Game.LoadMapDetails(RdlTagCollection.FromString(e.Result.TagString).GetTags<RdlTag>("MAP", "MAP"));

				// Display points on the map where the map details are defined.
				this.Dispatcher.BeginInvoke(() => this.LoadMaps());
			}
		}

		private void LoadMaps()
		{
            System.Diagnostics.Debug.WriteLine("Starting builder map downloads...");
            MapManager.BeginMapDownload(Game.Maps[Game.Maps.Count - 1].Name, true, 
				new MapManager.PlayerZoneDownloadCompleteCallback(this.LastMapLoadedCallback), 
				null,
				new MapManager.MapLoadedCallback(this.MapLoaded));

		}

		private void MapLoaded(int startX, int startY, List<RdlPlace> places)
		{
			_places.AddRange(places);
			this.LoadTiles();
		}

        private void LastMapLoadedCallback()
		{
			System.Diagnostics.Debug.WriteLine("LastMapLoadedCallback");
			_centerPosition = new Point3(512, 512, 0);
			ctlZoom.Value = ctlZoom.Minimum;
			this.MapLoadComplete(this, EventArgs.Empty);
        }

		private void LoadTiles()
		{
			System.Diagnostics.Debug.WriteLine("Places Count = {0}", _places.Count);
			int count = 0;
			foreach (var place in _places)
			{

				System.Diagnostics.Debug.WriteLine("Creating tile {0}, {1}, {2}", place.X, place.Y, place.Z);
				Tile tile = new Tile(place);
				tile.Click += new EventHandler<MouseButtonEventArgs>(tile_Click);
				tile.ActorDrop += new ActorEventHandler(tile_ActorDrop);
				_tiles.Add(tile);
				count++;
			}
			System.Diagnostics.Debug.WriteLine("Total tiles added = {0}", count + 1);
			this.RenderMap();
		}

		private void tile_ActorDrop(object sender, ActorEventArgs e)
		{
			Tile tile = sender as Tile;
			if (tile != null)
			{
                ctlWait.Show("Saving actor information...");
				e.Actor.OwnerID = tile.Place.ID;
				tile.Place.Actors.Add(e.Actor);
				this.PlaceChanged(new PlaceEventArgs(tile.Place));

				RdlTagCollection tags = new RdlTagCollection();
				tags.Add(e.Actor);
				tags.AddRange(e.Actor.Properties.ToArray());

				//BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				//client.SaveActorCompleted += new EventHandler<SaveActorCompletedEventArgs>(client_SaveActorCompleted);
				//client.SaveActorAsync(_token, tags.ToString());

				this.TileClick(tile);
			}
		}

		private void client_SaveActorCompleted(object sender, SaveActorCompletedEventArgs e)
		{
            try
            {
                if (e.Result.Success)
                {
                    RdlTagCollection tags = RdlTagCollection.FromString(e.Result.TagString);
                    if (tags != null)
                    {
                        foreach (var actor in tags.GetActors())
                        {
                            var tile = _tiles.Where(t => t.Place.ID == actor.OwnerID).FirstOrDefault();
                            if (tile != null)
                            {
                                tile.Place.Actors.Add(actor);
                                this.PlaceChanged(new PlaceEventArgs(tile.Place));
                            }
                        }
                    }
                }
            }
            finally
            {
                ctlWait.Hide();
            }
		}

		private void tile_Click(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			Tile tile = sender as Tile;
			this.TileClick(tile);
		}

		private void TileClick(Tile tile)
		{
			if (tile != null)
			{
				_centerPosition = tile.Location;
				this.SetSelectedTile(tile);

				this.SetView(_selectedTile.Location);

				cboTerrain.SelectedItem = Game.Terrain.Where(t => t.ID == tile.Place.Properties.GetValue<int>("Terrain")).FirstOrDefault();
				cboPlaceType.SelectedItem = new PlaceTypeDesc { Name = tile.Place.Properties.GetValue<string>("RuntimeType") };
				txtPlaceName.Text = tile.Place.Name;
				txtDescription.Text = tile.Place.Description;

				// Get the actors for the current place.
				//BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				//client.GetActorsCompleted += new EventHandler<GetActorsCompletedEventArgs>(client_GetActorsCompleted);
				//client.GetActorsAsync(_token, tile.Place.X, tile.Place.Y, tile.Place.Z);
			}
		}

		private void client_GetActorsCompleted(object sender, GetActorsCompletedEventArgs e)
		{
			if (e.Result.Success && _selectedTile != null)
			{
				_selectedTile.Place.Actors.Clear();
				_selectedTile.Place.Actors.AddRange(RdlTagCollection.FromString(e.Result.TagString).GetActors());
				this.PlaceChanged(new PlaceEventArgs(_selectedTile.Place));
			}
		}

		private void SetSelectedTile(Tile tile)
		{
			if (_selectedTile != null)
			{
				_selectedTile.Activated = false;
			}
			_selectedTile = tile;
			_selectedTile.Activated = true;
		}

		private void RenderMap()
		{
			System.Diagnostics.Debug.WriteLine("Rendering Map");
			this.ctlMap.Children.Clear();

			var tiles = _tiles.Where(t => t.Place.Z == _zIndex);
			foreach (var tile in tiles)
			{
				this.ctlMap.Children.Add(tile);
			}
		}

		public void SetView(Point3 center)
		{
			if (center.Z != _zIndex)
			{
				_zIndex = center.Z;
				this.RenderMap();
			}
			_centerPosition = center;

			this.mapTranslate.X = -((center.X * mapScale.ScaleX) * Tile.TileWidth) + ((_clip.Bounds.Width * 0.5));
			this.mapTranslate.Y = -((center.Y * mapScale.ScaleY) * Tile.TileHeight) + ((_clip.Bounds.Height * 0.5));
		}
		#endregion



		#region Map Editor Event Handlers
		private void cboTerrain_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Terrain terrain = cboTerrain.SelectedItem as Terrain;
			if (terrain != null)
			{
				ctlTerrain.Fill = Brushes.GetBrush(terrain.GetColor());
				_selectedTerrain = terrain;
			}
        }

        private void cboPlaceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			// Collapse all place specific elements.
			WildernessElement.Visibility = Visibility.Collapsed;

			// Show the selected place specific element.
			PlaceTypeDesc desc = cboPlaceType.SelectedItem as PlaceTypeDesc;
			if (desc != null)
			{
				if (desc.Name.Contains("Wilderness"))
				{
					WildernessElement.Visibility = Visibility.Visible;
				}
			}
        }

		private void txtPlaceName_TextChanged(object sender, TextChangedEventArgs e)
		{
			_selectedPlaceName = txtPlaceName.Text;
		}

		private void ctlViewPort_MouseMove(object sender, MouseEventArgs e)
		{
			Point point = e.GetPosition(ctlMapContainer);
			this.StatusUpdated(String.Format("X: {0}, Y: {1}, Z: {2}", 
				(int)(point.X / Tile.TileWidth), (int)(point.Y / Tile.TileHeight), _zIndex));

			if (_action == MapAction.Draw)
			{
				// Paint a square around the current tile.
				ctlHighlight.Children.Clear();
				ctlHighlight.Children.Add(this.CreateTileHighlight(
					(((int)(point.X / Tile.TileWidth)) * Tile.TileWidth),
					(((int)(point.Y / Tile.TileHeight)) * Tile.TileHeight),
					Tile.TileWidth, Tile.TileHeight));
			}
		}

		private UIElement CreateTileHighlight(double x, double y, double width, double height)
		{
			Rectangle obj = new Rectangle();
			obj.Width = width;
			obj.Height = height;
			obj.Stroke = Brushes.GetBrush(Colors.Yellow);
			obj.SetValue(Canvas.LeftProperty, x);
			obj.SetValue(Canvas.TopProperty, y);
			return obj;
		}

		private void SetAction(MapAction action)
		{
			this.SetMenuHighlight(bdrSelect, false);
			this.SetMenuHighlight(bdrDraw, false);
			this.SetMenuHighlight(bdrFill, false);
			this.SetMenuHighlight(bdrExits, false);

			ctlNav.Visibility = Visibility.Collapsed;

			_action = action;
			switch (_action)
			{
				case MapAction.Select:
					this.SetMenuHighlight(bdrSelect, true);
					break;
				case MapAction.Draw:
					this.SetMenuHighlight(bdrDraw, true);
					break;
				case MapAction.Fill:
					this.SetMenuHighlight(bdrFill, true);
					break;
				case MapAction.Exits:
					this.SetMenuHighlight(bdrExits, true);
					ctlNav.Visibility = Visibility.Visible;
					break;
			}
		}

		private void SetMenuHighlight(Border item, bool highlight)
		{
			if (highlight)
			{
				item.BorderBrush = Brushes.MenuHighlightBorderBrush;
				item.Background = Brushes.MenuHighlightBackgroundBrush;
			}
			else
			{
				item.BorderBrush = Brushes.BorderBrush;
				item.Background = Brushes.DialogFillBrush;
			}
		}

		private void MapPointsContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
            _isDragging = true;

			// Drag the map around.
			if (_action == MapAction.Select)
			{
				ctlHighlight.Children.Clear();

				this.ctlMapContainer.LostFocus += new RoutedEventHandler(MapPointsContainer_LostFocus);
				this.ctlMapContainer.LostMouseCapture += new MouseEventHandler(MapPointsContainer_LostMouseCapture);

				_mousePosition = e.GetPosition(null);
				_originalMousePosition = _mousePosition;

				if (!_isZoomIn && !_isZoomOut)
				{
					this.Cursor = Cursors.Hand;
				}

				this.ctlMapContainer.CaptureMouse();

				this.ctlMapContainer.MouseLeftButtonUp += new MouseButtonEventHandler(MapPointsContainer_MouseLeftButtonUp);
				this.ctlMapContainer.MouseMove += new MouseEventHandler(MapPointsContainer_MouseMove);
			}
			else if (_action == MapAction.Draw)
            {
                this.DrawTile(e.GetPosition(ctlMapContainer));
			}
            else if (_action == MapAction.Fill)
            {
                Point p = e.GetPosition(ctlMapContainer);
                this.DrawTile(new Point(p.X - 1, p.Y));
                this.DrawTile(new Point(p.X - 1, p.Y - 1));
                this.DrawTile(new Point(p.X, p.Y - 1));
                this.DrawTile(new Point(p.X + 1, p.Y - 1));
                this.DrawTile(new Point(p.X + 1, p.Y));
                this.DrawTile(new Point(p.X + 1, p.Y + 1));
                this.DrawTile(new Point(p.X, p.Y + 1));
                this.DrawTile(new Point(p.X - 1, p.Y + 1));
                this.DrawTile(p);
            }
		}

        private void DrawTile(Point mousePosition)
        {
            // See if a tile already exists.
            Tile tile = null;
            RdlPlace place = null;

            int x = (int)(mousePosition.X / Tile.TileWidth);
            int y = (int)(mousePosition.Y / Tile.TileHeight);

            var terrain = cboTerrain.SelectedItem as Terrain;
            var desc = cboPlaceType.SelectedItem as PlaceTypeDesc;

            tile = _tiles.Where(t => t.Location.X == x && t.Location.Y == y && t.Location.Z == _zIndex).FirstOrDefault();

            if (tile == null)
            {
                place = new RdlPlace(0, txtPlaceName.Text, x, y, _zIndex);

                tile = new Tile(place);
                _tiles.Add(tile);
            }
            else
            {
                place = tile.Place;
            }

            if (terrain != null) tile.Place.Properties.SetValue("Terrain", terrain.ID);
            if (desc != null) tile.Place.Properties.SetValue("RuntimeType", desc.Name);

			if (desc.Name.Contains("Wilderness"))
			{
				tile.Place.Properties.SetValue("CreatureMinLevel", txtMinCreatureLevel.Text);
				tile.Place.Properties.SetValue("CreatureMaxLevel", txtMaxCreatureLevel.Text);
			}

            tile.IsModified = true;
            tile.Refresh(place);

            this.RenderMap();
        }

		private void MapPointsContainer_MouseMove(object sender, MouseEventArgs e)
		{
            Point position = e.GetPosition(null);

            double deltaX = position.X - _mousePosition.X;
            double deltaY = position.Y - _mousePosition.Y;

            // TODO: If the mouse position is outside of the bounds of the map container then do not increment 
            // any of these values.

            // If the zoom buttons are pressed then just track the mouse position.
            if (!_isZoomIn && !_isZoomOut)
            {
                // Drag the map.
                mapTranslate.X += deltaX;
                mapTranslate.Y += deltaY;
                //_centerPosition.X -= (int)deltaX;
                //_centerPosition.Y -= (int)deltaY;
                //this.SetView(_centerPosition);
            }

            if (_dragCursor != null)
            {
                Point newPosition = new Point(
                    ((double)_dragCursor.GetValue(Canvas.LeftProperty)) + deltaX,
                    ((double)_dragCursor.GetValue(Canvas.TopProperty)) + deltaY);

                _dragCursor.SetValue(Canvas.LeftProperty, newPosition.X);
                _dragCursor.SetValue(Canvas.TopProperty, newPosition.Y);
            }

            _mousePosition = position;

			Point p = e.GetPosition(ctlMapContainer);
			_centerPosition = new Point3((int)(p.X / Tile.TileWidth), (int)(p.Y / Tile.TileHeight), _zIndex);
		}

		private void MapPointsContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).ReleaseMouseCapture();

			this.EndDrag();
		}

		private void MapPointsContainer_LostMouseCapture(object sender, MouseEventArgs e)
		{
			this.EndDrag();
		}

		private void MapPointsContainer_LostFocus(object sender, RoutedEventArgs e)
        {
            ((UIElement)sender).ReleaseMouseCapture();

			this.EndDrag();
		}

		private void EndDrag()
		{
			this.Cursor = Cursors.Arrow;
			this.ClearDragCursor();
			_isDragging = false;

			this.ctlMapContainer.LostFocus -= new RoutedEventHandler(MapPointsContainer_LostFocus);
			this.ctlMapContainer.LostMouseCapture -= new MouseEventHandler(MapPointsContainer_LostMouseCapture);

			this.ctlMapContainer.MouseLeftButtonUp -= new MouseButtonEventHandler(MapPointsContainer_MouseLeftButtonUp);
			this.ctlMapContainer.MouseMove -= new MouseEventHandler(MapPointsContainer_MouseMove);
        }

        private void ctlZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
			if (mapScale != null)
			{
				mapScale.ScaleX = mapScale.ScaleY = e.NewValue;
				this.SetView(_centerPosition);
			}
        }

		private void mnuSelect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.SetAction(MapAction.Select);
		}

		private void mnuDraw_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.SetAction(MapAction.Draw);
		}

		private void mnuFill_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.SetAction(MapAction.Fill);
		}

		private void mnuExits_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.SetAction(MapAction.Exits);
		}

		private void mnuZUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			_zIndex++;
			this.RenderMap();
		}

		private void mnuZDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
            _zIndex--;
            this.RenderMap();
		}

		private void mnuZoomIn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
            ctlZoom.Value += 0.1;
		}

		private void mnuZoomOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
            e.Handled = true;
            ctlZoom.Value -= 0.1;
		}

		private void mnuZoomToEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

            ctlZoom.Value = ctlZoom.Maximum;

            this.RenderMap();
		}

		private void mnuGridLines_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.MapLines.Visibility == Visibility.Visible)
			{
				this.SetMenuHighlight(bdrGridLines, false);
				this.MapLines.Visibility = Visibility.Collapsed;
				ToolTipService.SetToolTip(mnuGridLines, "Show Grid Lines");
			}
			else
			{
				this.SetMenuHighlight(bdrGridLines, true);
				this.MapLines.Visibility = Visibility.Visible;
				ToolTipService.SetToolTip(mnuGridLines, "Hide Grid Lines");
			}
		}

		private void mnuShowMaps_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_showMaps = !_showMaps;
			this.SetMenuHighlight(bdrShowMaps, _showMaps);
			ctlMapBorders.Children.Clear();
			if (_showMaps)
			{
				foreach (var map in Game.Maps)
				{
					MapBorder border = new MapBorder();
					border.Width = map.Width * Tile.TileWidth;
					border.Height = map.Height * Tile.TileHeight;
					border.SetValue(Canvas.LeftProperty, map.StartX * Tile.TileWidth);
					border.SetValue(Canvas.TopProperty, map.StartY * Tile.TileHeight);
					border.BorderResized += new EventHandler(border_BorderResized);
					ctlMapBorders.Children.Add(border);
				}
			}
		}

		private void border_BorderResized(object sender, EventArgs e)
		{
			MapBorder border = sender as MapBorder;
			Rect bounds = border.Bounds;
			var map = Game.Maps.Where(m => (bounds.X >= m.StartX && bounds.X <= m.EndX) && (bounds.Y >= m.StartY && bounds.Y <= m.EndY)).FirstOrDefault();
			if (map != null)
			{
				RdlTag tag = new RdlTag("MAP", "MAP");
				tag.Args.Add(map.Name);
				tag.Args.Add(map.Width);
				tag.Args.Add(map.Height);
				tag.Args.Add(map.StartX);
				tag.Args.Add(map.StartY);
				tag.Args.Add(map.EndX);
				tag.Args.Add(map.EndY);

				// TODO: Save Map
				//BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				//client.SaveMapCompleted += new EventHandler<SaveMapCompletedEventArgs>(client_SaveMapCompleted);
				//client.SaveMapAsync(_token, tag.ToString());
			}
		}

		private void client_SaveMapCompleted(object sender, SaveMapCompletedEventArgs e)
		{
		}

		private void SetDragCursor(string imageName, Size size, Point position)
		{
			Image img = new Image();
			img.Width = size.Width;
			img.Height = size.Height;
			img.Source = Asset.GetImageSource(new Uri(String.Concat(Asset.AssetPath, imageName)));
			this.SetDragCursor(img, position);
		}

		private void SetDragCursor(UIElement element, Point position)
		{
			if (_dragCursor != null) this.ClearDragCursor();
			_dragCursor = element;
			_dragCursor.SetValue(Canvas.LeftProperty, position.X);
			_dragCursor.SetValue(Canvas.TopProperty, position.Y);
			//this.PopupContainer.Children.Add(_dragCursor);
		}

		private void ClearDragCursor()
		{
			if (_dragCursor != null)
			{
				//this.PopupContainer.Children.Remove(_dragCursor);
			}
		}
		#endregion

        private int _placesToSaveCount = 0;
        private int _placesSavedCount = 0;
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			ctlWait.Show("Saving map informations...");
			var places = _tiles.Where(t => t.Place.ID == 0 || t.IsModified).Select(t => t.Place);
			RdlTagCollection tags = new RdlTagCollection();
            _placesToSaveCount = places.Count();
            _placesSavedCount = 0;
			foreach (var place in places)
			{
				tags.Add(place);
				tags.AddRange(place.Properties.ToArray());

				//BuilderServiceClient client = ServiceManager.CreateBuilderServiceClient();
				//client.SavePlacesCompleted += new EventHandler<SavePlacesCompletedEventArgs>(client_SavePlacesCompleted);
				//client.SavePlacesAsync(_token, tags.ToString(), place);
				tags = new RdlTagCollection();
			}
		}

		private void client_SavePlacesCompleted(object sender, SavePlacesCompletedEventArgs e)
		{
            _placesSavedCount++;
            if (_placesSavedCount >= _placesToSaveCount)
            {
                _placesSavedCount = 0;
                _placesToSaveCount = 0;
                ctlWait.Hide();
            }
			if (e.Result.Success)
			{
				// TODO: Display successful save.
				RdlTagCollection tags = RdlTagCollection.FromString(e.Result.TagString);
				RdlPlace place = e.UserState as RdlPlace;
				if (place != null)
				{
					var savedPlace = tags.GetPlaces().Where(p => p.X == place.X && p.Y == place.Y && p.Z == place.Z).FirstOrDefault();
					if (savedPlace != null)
					{
						place = savedPlace;
					}
					var tile = _tiles.Where(t => t.Location.X == place.X 
						&& t.Location.Y == place.Y 
						&& t.Location.Z == place.Z).FirstOrDefault();
					if (tile != null)
					{
						tile.IsModified = false;
						tile.Refresh(place);
					}
				}
			}
			else
			{
				// TODO: Display error message.
			}
		}

		private void txtDescription_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (_selectedTile != null)
			{
				_selectedTile.Place.Description = txtDescription.Text;
				_selectedTile.IsModified = true;
			}
		}
	}

	public delegate void PlaceEventHanlder(PlaceEventArgs e);
	public class PlaceEventArgs : EventArgs
	{
		public RdlPlace Place { get; set; }

		public PlaceEventArgs(RdlPlace place)
		{
			this.Place = place;
		}
	}

	public class PlaceTypeDesc
	{
		public string Name { get; set; }

		public string ShortName
		{
			get { return this.Name.Substring(this.Name.LastIndexOf('.') + 1); }
		}

		public override bool Equals(object obj)
		{
			PlaceTypeDesc desc = obj as PlaceTypeDesc;
			if (desc != null)
			{
				return this.Name.Equals(desc.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
	}
}