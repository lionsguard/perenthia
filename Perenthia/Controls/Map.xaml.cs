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
using Perenthia.Windows;
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class Map : UserControl, IWindow
	{
		public List<Tile> Tiles { get; private set; }
		private RdlTagCollection _tags = null;
		private Geometry _clip = null;
		private Tile _activeTile = null;
		private Dictionary<string, bool> _exits = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
		private int _zIndex = 0;

		private bool _isPressed = false;
		private bool _isMouseOver = false;
		private Point _mousePosition = new Point();

		private double SundialOffset = -48.511;

		public Tile ActiveTile
		{
			get { return _activeTile; }
		}

		public bool CanAlwaysMove
		{
			get { return (bool)GetValue(CanAlwaysMoveProperty); }
			set { SetValue(CanAlwaysMoveProperty, value); }
		}
		public static readonly DependencyProperty CanAlwaysMoveProperty = DependencyProperty.Register("CanAlwaysMove", typeof(bool), typeof(Map), new PropertyMetadata(false));
			

		public event DirectionEventHandler DirectionClick = delegate { };
		public event EventHandler TileNotFound = delegate { };
		public event EventHandler ActiveTileChanged = delegate { };

		public Map()
		{
			this.Loaded += new RoutedEventHandler(Map_Loaded);
			InitializeComponent();
		}

		void Map_Loaded(object sender, RoutedEventArgs e)
		{
			this.Tiles = new List<Tile>();
			_clip = new EllipseGeometry { Center = new Point(80, 81), RadiusX = 82, RadiusY = 82 };
			ctlViewPort.Clip = _clip;

			_exits.Add("North", false);
			_exits.Add("South", false);
			_exits.Add("East", false);
			_exits.Add("West", false);
			_exits.Add("Northeast", false);
			_exits.Add("Northwest", false);
			_exits.Add("Southeast", false);
			_exits.Add("Southwest", false);
			_exits.Add("Up", false);
			_exits.Add("Down", false);
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
			Logger.LogDebug("Loading {0} map tags into the map control.", places.Count);
			foreach (var place in places)
			{
				this.Tiles.Add(new Tile(place));
			}

			this.RenderMap();
		}

		private void RenderMap()
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
			this.mapTranslate.X = -(center.X * Tile.TileWidth) + ((_clip.Bounds.Width * 0.5) - 15);
			this.mapTranslate.Y = -(center.Y * Tile.TileHeight) + ((_clip.Bounds.Height * 0.5) - 15);

			//_center = new Point(center.X, center.Y);

			Tile tile = this.Tiles.FirstOrDefault(t => t.Location.Equals(center));
			if (tile != null)
			{
				if (_activeTile != null) _activeTile.Activated = false;
				_activeTile = tile;
				_activeTile.Activated = true;
				this.ActiveTileChanged(this, EventArgs.Empty);
				this.SetExits(tile.Place);
				this.HideLoading();
			}
			else
			{
				Logger.LogDebug("Tile not found at {0}: raising TileNotFound event.", center.ToString(true, true));
				this.ShowLoading();
				this.TileNotFound(this, EventArgs.Empty);
			}
			LocationLabel.Text = center.ToString(true, true);		
		}

		public Place FindPlace(Point3 location)
		{
			return this.Tiles.Where(t => t.Place != null && t.Place.Location == location).Select(t => t.Place).FirstOrDefault();
		}

		public void SetTime(RdlProperty time)
		{
			// Time property format:
			// 1:25 AM, Day 4 of Ebrum, Year 234 of the New Era.
			string timeString = time.GetValue<string>();
			int hours = Convert.ToInt32(timeString.Substring(0, 2).Replace(":", ""));

			if (timeString.Contains("PM")) hours += 12;

			// sundial rotation 360/24 == 15 degree rotation per hour.
			double rotation = (hours * 15);
			SundialRotateTransform.Angle = (rotation * -1) + SundialOffset;
			ToolTipService.SetToolTip(SundialImage, timeString);
		}

		private void SetExits(Place place)
		{
			_exits["North"] = place.Properties.GetValue<bool>("Exit_North");
			_exits["South"] = place.Properties.GetValue<bool>("Exit_South");
			_exits["East"] = place.Properties.GetValue<bool>("Exit_East");
			_exits["West"] = place.Properties.GetValue<bool>("Exit_West");
			_exits["Northeast"] = place.Properties.GetValue<bool>("Exit_Northeast");
			_exits["Northwest"] = place.Properties.GetValue<bool>("Exit_Northwest");
			_exits["Southeast"] = place.Properties.GetValue<bool>("Exit_Southeast");
			_exits["Southwest"] = place.Properties.GetValue<bool>("Exit_Southwest");
			_exits["Up"] = place.Properties.GetValue<bool>("Exit_Up");
			_exits["Down"] = place.Properties.GetValue<bool>("Exit_Down");

			north.Fill = place.Properties.GetValue<bool>("Exit_North") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			south.Fill = place.Properties.GetValue<bool>("Exit_South") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			east.Fill = place.Properties.GetValue<bool>("Exit_East") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			west.Fill = place.Properties.GetValue<bool>("Exit_West") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			northeast.Fill = place.Properties.GetValue<bool>("Exit_Northeast") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			northwest.Fill = place.Properties.GetValue<bool>("Exit_Northwest") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			southeast.Fill = place.Properties.GetValue<bool>("Exit_Southeast") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			southwest.Fill = place.Properties.GetValue<bool>("Exit_Southwest") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			up.Fill = place.Properties.GetValue<bool>("Exit_Up") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
			down.Fill = place.Properties.GetValue<bool>("Exit_Down") ? Brushes.DirectionOnBrush : Brushes.DirectionOffBrush;
		}

        public bool CanMove(string direction)
        {
			if (_exits.ContainsKey(direction))
			{
				return _exits[direction];
			}
			return false;
        }

		private void HandleDirectionClick(string direction)
		{
			if (this.CanMove(direction) || this.CanAlwaysMove)
			{
				this.DirectionClick(new DirectionEventArgs { Direction = direction });
			}
		}

		private void north_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("North");
		}

		private void south_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("South");
		}

		private void east_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("East");
		}

		private void west_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("West");
		}

		private void northwest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Northwest");
		}

		private void northeast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Northeast");
		}

		private void southwest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Southwest");
		}

		private void southeast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Southeast");
		}

		private void up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Up");
		}

		private void down_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleDirectionClick("Down");
		}

		private void MinimizeButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Minimize();
		}

		private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Visibility = Visibility.Collapsed;
		}

		private void LayoutRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_isMouseOver = true;
			Chrome.Visibility = Visibility.Visible;
		}

		private void LayoutRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_isMouseOver = false;
			Chrome.Visibility = Visibility.Collapsed;
		}

		private void Chrome_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_isPressed = true;
			BringToFront();

			_mousePosition = e.GetPosition(null);

			e.Handled = true;
			LayoutRoot.CaptureMouse();

			LayoutRoot.MouseLeftButtonUp += Chrome_MouseLeftButtonUp;
			LayoutRoot.MouseMove += Chrome_MouseMove;
		}

		private void Chrome_MouseMove(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(null);

			// Prevent the mouse from moving outside the bounds of the parent canvas.
			if (position.X <= 30) position.X = 30;
			if (position.Y <= 30) position.Y = 30;

			Canvas parent = this.Parent as Canvas;
			if (parent != null)
			{
				if (position.X >= (parent.Width - 30)) position.X = parent.Width - 30;
				if (position.Y >= (parent.Height - 30)) position.Y = parent.Height - 30;
			}

			double deltaX = position.X - _mousePosition.X;
			double deltaY = position.Y - _mousePosition.Y;

			Point newPosition = new Point(
				((double)this.GetValue(Canvas.LeftProperty)) + deltaX,
				((double)this.GetValue(Canvas.TopProperty)) + deltaY);

			this.SetValue(Canvas.LeftProperty, newPosition.X);
			this.SetValue(Canvas.TopProperty, newPosition.Y);

			_mousePosition = position;
		}

		private void Chrome_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_isPressed = false;

			LayoutRoot.ReleaseMouseCapture();

			LayoutRoot.MouseLeftButtonUp -= Chrome_MouseLeftButtonUp;
			LayoutRoot.MouseMove -= Chrome_MouseMove;
		}

		private void BringToFront()
		{
			var z = FloatableWindow.GetNextZ();
			this.SetValue(Canvas.ZIndexProperty, z);
		}
		#region IWindow Members

		public event EventHandler Minimized = delegate { };
		public event EventHandler Maximized = delegate { };

		public string WindowID { get; set; }

		public Point Position
		{
			get { return new Point(Canvas.GetLeft(this), Canvas.GetTop(this)); }
			set { Canvas.SetLeft(this, value.X); Canvas.SetTop(this, value.Y); }
		}

		public Size Size
		{
			get { return new Size(this.ActualWidth, this.ActualHeight); }
			set { this.Width = value.Width; this.Height = value.Height; }
		}

		public void Minimize()
		{
			this.Minimized(this, EventArgs.Empty);
		}

		public void Maximize()
		{
			this.Maximized(this, EventArgs.Empty);
		}

		#endregion
	}

	public delegate void DirectionEventHandler(DirectionEventArgs e);
	public class DirectionEventArgs : EventArgs
	{
		public string Direction { get; set; }	
	}
}
