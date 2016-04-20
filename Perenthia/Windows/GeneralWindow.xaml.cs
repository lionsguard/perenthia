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

namespace Perenthia.Windows
{
	public partial class GeneralWindow : FloatableWindow, IWindow
	{
		public GeneralWindow()
		{
			InitializeComponent();
		}
		public void Append(TextType type, string text, object tag, RoutedEventHandler linkCallback)
		{
			this.TextContainer.Append(type, text, tag, linkCallback);
		}

		#region IWindow Members


		public event EventHandler Maximized = delegate { };

		public string WindowID { get; set; }

		public Point Position
		{
			get { return GetPosition(); }
			set { Canvas.SetLeft(this, value.X); Canvas.SetTop(this, value.Y); }
		}

		public Size Size
		{
			get { return new Size(this.ActualWidth, this.ActualHeight); }
			set { this.Width = value.Width; this.Height = value.Height; }
		}

		public void Minimize()
		{
			this.OnMinimized(EventArgs.Empty);
		}

		public void Maximize()
		{
			this.Maximized(this, EventArgs.Empty);
		}

		#endregion
	}
}

