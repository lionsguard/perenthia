using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance.Markup;

namespace Radiance
{
	/// <summary>
	/// Represents a terrain in the virtual world.
	/// </summary>
	public class Terrain
	{
		/// <summary>
		/// Gets the default terrain instance.
		/// </summary>
		public static readonly Terrain None = new Terrain { ID = 0, Color = 0, Name = "None", WalkType = WalkTypes.None };

		/// <summary>
		/// Gets or sets the unique ID of the terrain.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Gets or sets the name of the terrain.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the value indicating the walk type of the terrain.
		/// </summary>
		public WalkTypes WalkType { get; set; }

		/// <summary>
		/// Gets or sets the color of the terrain.
		/// </summary>
		public int Color { get; set; }

		/// <summary>
		/// Gets or sets the virtual image url of the terrain.
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// Initializes the terrain instance from the specified RdlTerrain tag.
		/// </summary>
		/// <param name="terrain">The RdlTerrain tag instance.</param>
		public void Initialize(RdlTerrain terrain)
		{
			this.ID = terrain.ID;
			this.Name = terrain.Name;
			this.Color = terrain.Color;
			this.WalkType = (WalkTypes)terrain.WalkType;
			this.ImageUrl = terrain.ImageUrl;
		}

		/// <summary>
		/// Gets a string representation of the terrain.
		/// </summary>
		/// <returns>The string representation of the terrain.</returns>
		public override string ToString()
		{
			return this.Name;
		}
	}

	/// <summary>
	/// Represents a collection of Terrain instances with the key being the ID of the terrain and the value the Terrian instance.
	/// </summary>
	public class TerrainDictionary : Dictionary<int, Terrain>
	{
		/// <summary>
		/// Gets the Terrain instance with the specified key.
		/// </summary>
		/// <param name="key">The key of the Terrain to retrieve.</param>
		/// <returns>An instance of Terrain for the specified key or Terrain.None if the key was not found.</returns>
		public new Terrain this[int key]
		{
			get
			{
				if (this.ContainsKey(key))
				{
					return base[key];
				}
				return Terrain.None;
			}
			set
			{
				if (this.ContainsKey(key))
				{
					base[key] = value;
				}
				else
				{
					base.Add(key, value);
				}
			}
		}

		/// <summary>
		/// Initializes the terrain collection with the values from the RdlTerrain tags.
		/// </summary>
		/// <param name="terrain">The tags that contain the terrain for the collection.</param>
		public void Initialize(RdlTerrain[] terrain)
		{
			foreach (var item in terrain)
			{
				Terrain t = new Terrain();
				t.Initialize(item);
				this.Add(t.ID, t);
			}
		}
	}
}
