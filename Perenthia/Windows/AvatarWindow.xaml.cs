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
using Perenthia.Controls;
using Perenthia.Models;

namespace Perenthia.Windows
{
	public partial class AvatarWindow : UserControl, IWindow
	{
		private bool _isPressed = false;
		private bool _isMouseOver = false;
		private Point _mousePosition = new Point();

		public event EventHandler Click = delegate { };
		public event EventHandler DragCompleted = delegate { };

		public Avatar Avatar
		{
			get { return this.DataContext as Avatar; }
			set
			{
				this.DataContext = null;

				if (value != null)
				{
					this.DataContext = value;

					this.StatHealth.Maximum = value.BodyMax;
					this.StatHealth.Value = value.Body;

					this.StatWillpower.Maximum = value.MindMax;
					this.StatWillpower.Value = value.Mind;
				}
			}
		}

		public bool EnableAsButton
		{
			get { return (bool)GetValue(EnableAsButtonProperty); }
			set { SetValue(EnableAsButtonProperty, value); }
		}
		public static readonly DependencyProperty EnableAsButtonProperty =
			DependencyProperty.Register("EnableAsButton", typeof(bool), typeof(AvatarWindow), new PropertyMetadata(false));

		public bool EnableDetails
		{
			get { return (bool)GetValue(EnableDetailsProperty); }
			set { SetValue(EnableDetailsProperty, value); }
		}
		public static readonly DependencyProperty EnableDetailsProperty = DependencyProperty.Register("EnableDetails", typeof(bool), typeof(AvatarWindow), new PropertyMetadata(false));

		public bool EnableAffects
		{
			get { return (bool)GetValue(EnableAffectsProperty); }
			set { SetValue(EnableAffectsProperty, value); }
		}
		public static readonly DependencyProperty EnableAffectsProperty = DependencyProperty.Register("EnableAffects", typeof(bool), typeof(AvatarWindow), new PropertyMetadata(false));

		public bool HasCloseButton
		{
			get { return (bool)GetValue(HasCloseButtonProperty); }
			set { SetValue(HasCloseButtonProperty, value); }
		}
		public static readonly DependencyProperty HasCloseButtonProperty = DependencyProperty.Register("HasCloseButton", typeof(bool), typeof(AvatarWindow), new PropertyMetadata(true, new PropertyChangedCallback(AvatarWindow.OnHasCloseButtonPropertyChanged)));
		private static void OnHasCloseButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var panel = obj as AvatarWindow;
			if (panel == null) return;

			panel.CloseButton.Visibility = panel.HasCloseButton ? Visibility.Visible : Visibility.Collapsed;
		}

		public bool EnableCurrencyDisplay
		{
			get { return (bool)GetValue(EnableCurrencyDisplayProperty); }
			set { SetValue(EnableCurrencyDisplayProperty, value); }
		}
		public static readonly DependencyProperty EnableCurrencyDisplayProperty = DependencyProperty.Register("EnableCurrencyDisplay", typeof(bool), typeof(AvatarWindow), new PropertyMetadata(false, new PropertyChangedCallback(AvatarWindow.OnEnableCurrencyDisplayPropertyChanged)));
		private static void OnEnableCurrencyDisplayPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var panel = obj as AvatarWindow;
			if (panel == null) return;

			panel.CurrencyContainer.Visibility = panel.EnableCurrencyDisplay ? Visibility.Visible : Visibility.Collapsed;
		}
            

		public AvatarWindow()
		{
			InitializeComponent();
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
			GoToState(true);
		}

		private void LayoutRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_isMouseOver = false;
			GoToState(true);
		}

		private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_isPressed = true;
			GoToState(true);
			BringToFront();

			Click(this, EventArgs.Empty);

			_mousePosition = e.GetPosition(null);

			e.Handled = true;
			LayoutRoot.CaptureMouse();

			LayoutRoot.MouseLeftButtonUp += LayoutRoot_MouseLeftButtonUp;
			LayoutRoot.MouseMove += LayoutRoot_MouseMove;
		}

		private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
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

		private void LayoutRoot_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_isPressed = false;
			GoToState(true);

			LayoutRoot.ReleaseMouseCapture();

			LayoutRoot.MouseLeftButtonUp -= LayoutRoot_MouseLeftButtonUp;
			LayoutRoot.MouseMove -= LayoutRoot_MouseMove;

			DragCompleted(this, EventArgs.Empty);
		}

		private void BringToFront()
		{
			var z = FloatableWindow.GetNextZ();
			this.SetValue(Canvas.ZIndexProperty, z);
		}

		private void GoToState(bool useTransitions)
		{
			// Common States
			if (_isPressed)
			{
				VisualStateManager.GoToState(this, "Pressed", useTransitions);
			}
			else if (_isMouseOver)
			{
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Normal", useTransitions);
			}
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

		public void RefreshAffects()
		{
			if (this.Avatar == null)
				return;

			if (this.EnableAffects)
			{
				// Set Affect icons.
				var affects = this.Avatar.Properties.Values.Where(p => p.Name.StartsWith("Affect_"));
				BuffsList.Children.Clear();
				foreach (var affect in affects)
				{
					AffectIcon icon = new AffectIcon(this.Avatar);
					icon.Affect = affect;

					BuffsList.Children.Add(icon);
				}
			}
		}

		public void RefreshAwards()
		{
			if (this.Avatar != null)
			{
				var awards = this.Avatar.Properties.Values.Where(p => p.Name.StartsWith("Award_"));
				// TODO: Where are awards going to go?
			}
		}
	}
}
