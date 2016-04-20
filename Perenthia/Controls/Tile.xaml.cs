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

using Lionsguard;
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class Tile : UserControl, IDropContainer
	{
		public static readonly double TileWidth = 22;
		public static readonly double TileHeight = 22;

		public event EventHandler<MouseButtonEventArgs> Click = delegate { };
		public event ActorEventHandler ActorDrop = delegate { };

		public bool IsModified
		{
			get { return (bool)GetValue(IsModifiedProperty); }
			set { SetValue(IsModifiedProperty, value); }
		}
		public static readonly DependencyProperty IsModifiedProperty = DependencyProperty.Register("IsModified", typeof(bool), typeof(Tile), new PropertyMetadata(false));

		public bool Activated
		{
			get { return (bool)GetValue(ActivatedProperty); }
			set { SetValue(ActivatedProperty, value); }
		}
		public static readonly DependencyProperty ActivatedProperty = DependencyProperty.Register("Activated", typeof(bool), typeof(Tile), new PropertyMetadata(false, new PropertyChangedCallback(Tile.OnActivatedPropertyChanged)));

		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(Tile), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(Tile.OnFillPropertyChanged)));


		public Point RenderLocation
		{
			get { return new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty)); }
			set
			{
				this.SetValue(Canvas.LeftProperty, value.X);
				this.SetValue(Canvas.TopProperty, value.Y);
			}
		}

		public Point3 Location
		{
			get { return new Point3(this.Place.X, this.Place.Y, this.Place.Z); }
		}

		public Place Place { get; private set; }	

		public Tile()
		{
            this.Loaded += new RoutedEventHandler(Tile_Loaded);
			InitializeComponent();
			this.Place = new Place();
		}

		public Tile(RdlPlace place) : this()
		{
            this.Refresh(place);
		}

        private void Tile_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Width == Double.NaN)
            {
                this.Width = TileWidth;
            }
            if (this.Height == Double.NaN)
            {
                this.Height = TileHeight;
            }
        }

        public void Refresh(RdlPlace place)
        {
            this.Place = new Place(place);
            this.RenderLocation = new Point(this.Place.X * this.Width, this.Place.Y * this.Height);
            this.SetExits();

            var terrain = Game.Terrain.Where(t => t.ID == place.Properties.GetValue<int>("Terrain")).FirstOrDefault();
            if (terrain != null)
            {
				this.Fill = Brushes.GetBrush(terrain.GetColor());
            }
        }

		private static void OnFillPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Tile).TileElement.Background = (obj as Tile).Fill;
		}

		private static void OnActivatedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Tile).SetActivation();
		}

		private void SetActivation()
		{
			player.Visibility = (this.Activated) ? Visibility.Visible : Visibility.Collapsed;
		}

		public void RefreshExits()
		{
			this.SetExits();
		}

		private void SetExits()
		{
			north.Visibility = this.Place.Properties.GetValue<bool>("Exit_North") ? Visibility.Visible : Visibility.Collapsed;
			south.Visibility = this.Place.Properties.GetValue<bool>("Exit_South") ? Visibility.Visible : Visibility.Collapsed;
			east.Visibility = this.Place.Properties.GetValue<bool>("Exit_East") ? Visibility.Visible : Visibility.Collapsed;
			west.Visibility = this.Place.Properties.GetValue<bool>("Exit_West") ? Visibility.Visible : Visibility.Collapsed;
			northeast.Visibility = this.Place.Properties.GetValue<bool>("Exit_Northeast") ? Visibility.Visible : Visibility.Collapsed;
			northwest.Visibility = this.Place.Properties.GetValue<bool>("Exit_Northwest") ? Visibility.Visible : Visibility.Collapsed;
			southeast.Visibility = this.Place.Properties.GetValue<bool>("Exit_Southeast") ? Visibility.Visible : Visibility.Collapsed;
			southwest.Visibility = this.Place.Properties.GetValue<bool>("Exit_Southwest") ? Visibility.Visible : Visibility.Collapsed;
			//up.Visibility = this.Place.Properties.GetValue<bool>("Exit_Up") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			//down.Visibility = this.Place.Properties.GetValue<bool>("Exit_Down") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
		}

		private void TileElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Click(this, e);
		}

		#region IDropContainer Members

		public void Drop(IDroppable item)
		{
			if (item is IActorListItem)
			{
				Logger.LogDebug("Dropped IActorListItem onto tile...");
				this.ActorDrop(this, new ActorEventArgs { Actor = (item as IActorListItem).Actor });
			}
		}

		#endregion
	}
}
