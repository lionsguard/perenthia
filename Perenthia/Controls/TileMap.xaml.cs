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

using Radiance;
using Radiance.Markup;

namespace Perenthia.Controls
{
	public partial class TileMap : UserControl
	{
		public List<Tile> Tiles { get; private set; }
		private RdlTagCollection _tags = null;
		private Tile _activeTile = null;
		private int _zIndex = 0;

		public Geometry ViewPortClip
		{
			get { return (Geometry)GetValue(ViewPortClipProperty); }
			set { SetValue(ViewPortClipProperty, value); }
		}
		public static readonly DependencyProperty ViewPortClipProperty = DependencyProperty.Register("ViewPortClip", typeof(Geometry), typeof(TileMap), new PropertyMetadata(null, new PropertyChangedCallback(TileMap.OnViewPortClipPropertyChanged)));
		private static void OnViewPortClipPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as TileMap).ctlViewPort.SetValue(UIElement.ClipProperty, e.NewValue);
		}
			

		public TileMap()
		{
			this.Tiles = new List<Tile>();
			this.Loaded += new RoutedEventHandler(TileMap_Loaded);
			InitializeComponent();
		}

		private void TileMap_Loaded(object sender, RoutedEventArgs e)
		{
		}

		public void ShowLoading()
		{
			ctlLoading.Visibility = Visibility.Visible;
		}

		public void HideLoading()
		{
			ctlLoading.Visibility = Visibility.Collapsed;
		}
		public RdlTagCollection GetTags()
		{
			return _tags;
		}
		public void LoadMap(RdlTagCollection tags)
		{
			_tags = tags;
			this.Tiles.Clear();

			List<RdlPlace> places = tags.GetObjects<RdlPlace>();
			foreach (var place in places)
			{
				Tile tile = new Tile(place);

				//var terrain = Game.Terrain.Where(t => t.ID == place.Properties.GetValue<int>("Terrain")).FirstOrDefault();
				//if (terrain != null)
				//{
				//    Color color = new Color();
				//    color.A = (byte)((terrain.Color & -16777216) >> 0x18);
				//    color.R = (byte)((terrain.Color & 0xff0000) >> 0x10);
				//    color.G = (byte)((terrain.Color & 0xff00) >> 8);
				//    color.B = (byte)(terrain.Color & 0xff);

				//    tile.Fill = Brushes.GetBrush(color);
				//}

				this.Tiles.Add(tile);
			}

			this.RenderMap();
		}

		public void RenderMap()
		{
			this.ctlMap.Children.Clear();

			var tiles = this.Tiles.Where(t => t.Place.Z == _zIndex);
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

			// The player icon does not move, the map and avatar container move under them.
			this.mapTranslate.X = -(center.X * Tile.TileWidth) + ((this.ViewPortClip.Bounds.Width * 0.5) - 15);
			this.mapTranslate.Y = -(center.Y * Tile.TileHeight) + ((this.ViewPortClip.Bounds.Height * 0.5) - 15);

			//_center = new Point(center.X, center.Y);

			Tile tile = this.Tiles.FirstOrDefault(t => t.Location.Equals(center));
			if (tile != null)
			{
				if (_activeTile != null) _activeTile.Activated = false;
				_activeTile = tile;
				_activeTile.Activated = true;
				this.HideLoading();
			}
			else
			{
				this.ShowLoading();
				//this.TileNotFound(this, EventArgs.Empty);
			}
		}
	}
}
