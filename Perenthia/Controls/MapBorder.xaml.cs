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

namespace Perenthia.Controls
{
	public partial class MapBorder : UserControl
	{
		Point _mousePosition = new Point();

		public event EventHandler BorderResized = delegate { };

		public Rect Bounds
		{
			get
			{
				double x = (double)this.GetValue(Canvas.LeftProperty);
				double y = (double)this.GetValue(Canvas.TopProperty);
				return new Rect(x, y, this.Width, this.Height);
			}
		}

		public MapBorder()
		{
			InitializeComponent();
		}

		private void ctlBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;

			ctlBorder.LostFocus += new RoutedEventHandler(ctlBorder_LostFocus);
			ctlBorder.LostMouseCapture += new MouseEventHandler(ctlBorder_LostMouseCapture);

			_mousePosition = e.GetPosition(null);

			ctlBorder.CaptureMouse();
			ctlBorder.MouseLeftButtonUp += new MouseButtonEventHandler(ctlBorder_MouseLeftButtonUp);
			ctlBorder.MouseMove += new MouseEventHandler(ctlBorder_MouseMove);
		}

		private void ctlBorder_MouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(null);

			Point delta = new Point(position.X - _mousePosition.X, position.Y - _mousePosition.Y);

			double x = (double)this.GetValue(Canvas.LeftProperty);
			double y = (double)this.GetValue(Canvas.TopProperty);

			if (delta.Y > 0)
			{
				// Moving down
				y += delta.Y;
			}
			else if (delta.Y < 0)
			{
				// Moving up
				this.Height += delta.Y;
			}

			if (delta.X > 0)
			{
				// Moving right
				x += delta.X;
			}
			else if (delta.X < 0)
			{
				// Moving left
				this.Width += delta.X;
			}

			this.SetValue(Canvas.LeftProperty, x);
			this.SetValue(Canvas.TopProperty, y);
			_mousePosition = position;
		}

		private void ctlBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			(sender as FrameworkElement).ReleaseMouseCapture();

			this.EndDrag();
		}

		private void ctlBorder_LostMouseCapture(object sender, MouseEventArgs e)
		{
			this.EndDrag();
		}

		private void ctlBorder_LostFocus(object sender, RoutedEventArgs e)
		{
			this.EndDrag();
		}

		private void EndDrag()
		{
			this.BorderResized(this, EventArgs.Empty);
		}

		private void ctlBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			Point p = e.GetPosition(ctlBorder);
			if (p.Y <= 2 || p.Y >= (this.Height - 2))
			{
				ctlBorder.Cursor = Cursors.SizeNS;
				//if (p.X <= 2 || p.X >= (this.Width - 2))
				//{
				//    ctlBorder.Cursor = Cursors.SizeNS;
				//}
				//else
				//{
				//    ctlBorder.Cursor = Cursors.SizeNS;
				//}
			}
			else
			{
				ctlBorder.Cursor = Cursors.SizeWE;
			}
		}

		private void ctlBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			ctlBorder.Cursor = Cursors.Arrow;
		}
	}
}
