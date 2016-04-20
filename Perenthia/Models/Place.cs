using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Radiance;
using Radiance.Markup;
using System.Collections.Generic;
using System.Linq;

namespace Perenthia.Models
{
	public class Place : Actor
	{
		#region Properties
		public List<Actor> Actors { get; private set; }

		public int X
		{
			get { return this.Properties.GetValue<int>(XProperty); }
			set { this.Properties.SetValue(XProperty, value); }
		}
		public static readonly string XProperty = "X";

		public int Y
		{
			get { return this.Properties.GetValue<int>(YProperty); }
			set { this.Properties.SetValue(YProperty, value); }
		}
		public static readonly string YProperty = "Y";

		public int Z
		{
			get { return this.Properties.GetValue<int>(ZProperty); }
			set { this.Properties.SetValue(ZProperty, value); }
		}
		public static readonly string ZProperty = "Z";

		/// <summary>
		/// Gets or sets the Terrain property value.
		/// </summary>	
		public int Terrain
		{
			get { return Properties.GetValue<int>(TerrainPropertyName); }
			set { Properties.SetValue(TerrainPropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the Terrain property as stored in the object property collection.
		/// </summary>
		public const string TerrainPropertyName = "Terrain";

		/// <summary>
		/// Gets or sets the TypeName property value.
		/// </summary>
		public string TypeName	
		{
			get { return Properties.GetValue<string>(TypeNamePropertyName); }
			set { Properties.SetValue(TypeNamePropertyName, value); }
		}
		/// <summary>
		/// Gets the name of the TypeName property as stored in the object property collection.
		/// </summary>
		public const string TypeNamePropertyName = "TypeName";

		public Color TerrainColor
		{
			get
			{
				if (Game.Terrain != null)
				{
					var terrain = Game.Terrain.Where(t => t.ID == this.Terrain).FirstOrDefault();
					if (terrain != null)
					{
						return terrain.GetColor();
					}
				}
				return Colors.Black;
			}
		}

		public Point3 Location
		{
			get { return new Point3(X, Y, Z); }
		}
		public string LocationString
		{
			get { return this.Location.ToString(true, true); }
		}
		#endregion

		public Place()
			: base()
		{
			Init();
		}

		public Place(RdlPlace tag)
			: base(tag)
		{
			Init();

			// Actors
			foreach (var actor in tag.Actors)
			{
				this.Actors.Add(new Actor(actor));
			}
		}

		private void Init()
		{
			this.Actors = new List<Actor>();
		}
	}
}
