using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Lionsguard
{
	[TemplatePart(Name = "RootElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "ContentElement", Type = typeof(ContentControl))]
	[TemplatePart(Name = "TitleBarElement", Type = typeof(Border))]
	[TemplatePart(Name = "CloseButtonElement", Type = typeof(Button))]
	[TemplatePart(Name = "TitleLabelElement", Type = typeof(TextBlock))]
	[ContentProperty("Content")]
	public class Window : Control
	{
		public FrameworkElement RootElement { get; set; }
		private ContentControl ContentElement { get; set; }
		private Border TitleBarElement { get; set; }
		private Button CloseButtonElement { get; set; }
		private TextBlock TitleLabelElement { get; set; }

		private Point MousePosition { get; set; }

		/// <summary>
		/// Gets or sets the Content of the Window.
		/// </summary>
		public UIElement Content
		{
			get { return (UIElement)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(Window), new PropertyMetadata(null, new PropertyChangedCallback(Window.OnContentPropertyChanged)));
		private static void OnContentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Window).SetControlValues();
		}

		/// <summary>
		/// Gets or sets the Title displayed in the title bar of the window.
		/// </summary>
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Window), new PropertyMetadata("Title", new PropertyChangedCallback(Window.OnTitlePropertyChanged)));
		private static void OnTitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Window).SetControlValues();
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the close button of the title bar should be displayed.
		/// </summary>
		public bool ShowCloseButton
		{
			get { return (bool)GetValue(ShowCloseButtonProperty); }
			set { SetValue(ShowCloseButtonProperty, value); }	
		}
		public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(Window), new PropertyMetadata(true, new PropertyChangedCallback(Window.OnShowCloseButtonPropertyChanged)));
		private static void OnShowCloseButtonPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Window).SetControlValues();
		}

		/// <summary>
		/// Gets a value indicating whether or not the window is currently open and visible.
		/// </summary>
		public bool IsOpen
		{
			get { return this.Visibility == Visibility.Visible; }
		}

		/// <summary>
		/// An event that is raised when the window is closed.
		/// </summary>
		public event EventHandler Closed = delegate { };

		/// <summary>
		/// Initializes a new instance of the Lionsguard.Window class.
		/// </summary>
		public Window()
		{
			this.DefaultStyleKey = typeof(Window);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.RootElement = base.GetTemplateChild("RootElement") as FrameworkElement;
			this.ContentElement = base.GetTemplateChild("ContentElement") as ContentControl;
			this.TitleBarElement = base.GetTemplateChild("TitleBarElement") as Border;
			this.CloseButtonElement = base.GetTemplateChild("CloseButtonElement") as Button;
			this.TitleLabelElement = base.GetTemplateChild("TitleLabelElement") as TextBlock;
			
			if (this.RootElement != null)
			{
				this.RootElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnRootElementMouseLeftButtonDown);

				this.TitleBarElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnTitleBarMouseLeftButtonDown);
				this.TitleBarElement.MouseEnter += new MouseEventHandler(OnTitleBarMouseEnter);
				this.TitleBarElement.MouseLeave += new MouseEventHandler(OnTitleBarMouseLeave);

				this.TitleLabelElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnTitleBarMouseLeftButtonDown);
				this.TitleLabelElement.MouseEnter += new MouseEventHandler(OnTitleBarMouseEnter);
				this.TitleLabelElement.MouseLeave += new MouseEventHandler(OnTitleBarMouseLeave);

				this.CloseButtonElement.Click += new RoutedEventHandler(OnCloseButtonClick);

				this.SetControlValues();
			}
		}

		private void SetControlValues()
		{
			if (this.ContentElement != null)
			{
				this.ContentElement.Content = this.Content;
			}
			if (this.TitleLabelElement != null)
			{
				this.TitleLabelElement.Text = this.Title;
				ToolTipService.SetToolTip(this.TitleLabelElement, this.Title);
			}
			if (this.CloseButtonElement != null)
			{
				if (this.ShowCloseButton) this.CloseButtonElement.Visibility = Visibility.Visible;
				else this.CloseButtonElement.Visibility = Visibility.Collapsed;
			}
		}

		private void OnRootElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Set the Z-Index of the window to the top most position.
			this.BringToFront();
		}

		private void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void OnTitleBarMouseLeave(object sender, MouseEventArgs e)
		{
			this.Cursor = Cursors.Arrow;
		}

		private void OnTitleBarMouseEnter(object sender, MouseEventArgs e)
		{
			this.Cursor = Cursors.Hand;
		}

		private void OnTitleBarMouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(null);

			// Prevent the mouse from moving outside the bounds of the parent canvas.
			if (position.X <= 30) position.X = 30;
			if (position.Y <= 30) position.Y = 30;

			Canvas parent = this.Parent as Canvas;
			if (parent != null)
			{
				if (position.X >= (parent.Width - 30)) position.X = parent.Width - 30;
				if (position.Y >= (parent.Height - 30)) position.Y = parent.Height - 30;
			}

			Logger.LogDebug("position = {0}", position);

			double deltaX = position.X - this.MousePosition.X;
			double deltaY = position.Y - this.MousePosition.Y;
			
			Point newPosition = new Point(
				((double)this.GetValue(Canvas.LeftProperty)) + deltaX,
				((double)this.GetValue(Canvas.TopProperty)) + deltaY);

			this.SetValue(Canvas.LeftProperty, newPosition.X);
			this.SetValue(Canvas.TopProperty, newPosition.Y);

			this.MousePosition = position;
		}

		private void OnTitleBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).ReleaseMouseCapture();

			this.Cursor = Cursors.Arrow;

			this.TitleBarElement.MouseLeftButtonUp -= new MouseButtonEventHandler(OnTitleBarMouseLeftButtonUp);
			this.TitleBarElement.MouseMove -= new MouseEventHandler(OnTitleBarMouseMove);
			this.TitleLabelElement.MouseLeftButtonUp -= new MouseButtonEventHandler(OnTitleBarMouseLeftButtonUp);
			this.TitleLabelElement.MouseMove -= new MouseEventHandler(OnTitleBarMouseMove);
		}

		private void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.MousePosition = e.GetPosition(null);
			this.Cursor = Cursors.Hand;

			((UIElement)sender).CaptureMouse();

			this.TitleBarElement.MouseLeftButtonUp += new MouseButtonEventHandler(OnTitleBarMouseLeftButtonUp);
			this.TitleBarElement.MouseMove += new MouseEventHandler(OnTitleBarMouseMove);
			this.TitleLabelElement.MouseLeftButtonUp += new MouseButtonEventHandler(OnTitleBarMouseLeftButtonUp);
			this.TitleLabelElement.MouseMove += new MouseEventHandler(OnTitleBarMouseMove);
		}

		/// <summary>
		/// Causes the current window to be re-z-indexed to the top most window.
		/// </summary>
		public void BringToFront()
		{
			Canvas.SetZIndex(this, FloatableWindow.GetNextZ());
			//// Search the top most "Window" and swap z-indexes.
			//Panel parent = this.Parent as Panel; // Panel is the base for Canvas, Grid and StackPanel.
			//if (parent != null)
			//{
			//    int currentZIndex = (int)this.GetValue(Canvas.ZIndexProperty);
			//    var child = (from c in parent.Children where c is Window select c as Window).OrderByDescending(c => (int)c.GetValue(Canvas.ZIndexProperty)).FirstOrDefault();
			//    if (child != null)
			//    {
			//        int topZIndex = (int)child.GetValue(Canvas.ZIndexProperty);
			//        if (topZIndex == 0) topZIndex = 1; // If the value has not been set then just default it to 1.
			//        if (topZIndex > currentZIndex)
			//        {
			//            this.SetValue(Canvas.ZIndexProperty, topZIndex);
			//            child.SetValue(Canvas.ZIndexProperty, currentZIndex);
			//        }
			//    }
			//}
		}

		public void Show()
		{
			this.Visibility = Visibility.Visible;
			// If content is present show that as well.
			if (this.Content != null) this.Content.Visibility = Visibility.Visible;
			BringToFront();
		}

		public void Close()
		{
			this.Visibility = Visibility.Collapsed;
			// If content is present hide that as well.
			if (this.Content != null) this.Content.Visibility = Visibility.Collapsed;
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
