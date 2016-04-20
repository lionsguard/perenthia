using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Perenthia.Dialogs
{
	public partial class DialogWindow : UserControl
	{
		private Point MousePosition { get; set; }

		public string Title
		{
			get { return this.TitleLabel.Text; }
			set { this.TitleLabel.Text = value; }
		}

		public bool ShowCloseButton
		{
			get { return this.CloseButton.Visibility == Visibility.Visible; }
			set
			{
				if (value) this.CloseButton.Visibility = Visibility.Visible;
				else this.CloseButton.Visibility = Visibility.Collapsed;
			}
		}

		public UIElementCollection DialogContent
		{
			get { return this.WindowContent.Children; }
		}

		public event EventHandler Closed = delegate { };

		public DialogWindow()
		{
			this.Loaded += new RoutedEventHandler(DialogWindow_Loaded);
			InitializeComponent();
		}

		private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(TitleBarMouseLeftButtonDown);
			this.TitleBar.MouseEnter += new MouseEventHandler(TitleBarMouseEnter);
			this.TitleBar.MouseLeave += new MouseEventHandler(TitleBarMouseLeave);

			this.CloseButton.Click += new RoutedEventHandler(CloseButton_Click);
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void TitleBarMouseLeave(object sender, MouseEventArgs e)
		{
			this.Cursor = Cursors.Arrow;
		}

		private void TitleBarMouseEnter(object sender, MouseEventArgs e)
		{
			this.Cursor = Cursors.Hand;
		}

		private void TitleBarMouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(null);

			double deltaX = position.X - this.MousePosition.X;
			double deltaY = position.Y - this.MousePosition.Y;

			Point newPosition = new Point(
				((double)this.GetValue(Canvas.LeftProperty)) + deltaX,
				((double)this.GetValue(Canvas.TopProperty)) + deltaY);

			this.SetValue(Canvas.LeftProperty, newPosition.X);
			this.SetValue(Canvas.TopProperty, newPosition.Y);

			this.MousePosition = position;
		}

		private void TitleBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).ReleaseMouseCapture();

			this.Cursor = Cursors.Arrow;

			this.TitleBar.MouseLeftButtonUp -= new MouseButtonEventHandler(TitleBarMouseLeftButtonUp);
			this.TitleBar.MouseMove -= new MouseEventHandler(TitleBarMouseMove);
		}

		private void TitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.MousePosition = e.GetPosition(null);
			this.Cursor = Cursors.Hand;

			((UIElement)sender).CaptureMouse();

			this.TitleBar.MouseLeftButtonUp += new MouseButtonEventHandler(TitleBarMouseLeftButtonUp);
			this.TitleBar.MouseMove += new MouseEventHandler(TitleBarMouseMove);
		}

		public void Show()
		{
			this.Visibility = Visibility.Visible;
		}

		public void Close()
		{
			this.Visibility = Visibility.Collapsed;
			this.Closed(this, EventArgs.Empty);
		}

		public void ToggleWindow()
		{
			if (this.Visibility == Visibility.Visible)
			{
				this.Close();
			}
			else
			{
				this.Show();
			}
		}
	}
}
