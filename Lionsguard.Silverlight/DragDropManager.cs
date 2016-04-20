using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Lionsguard
{
    /// <summary>
    /// Represents a class used to manage drag and drop items.
    /// </summary>
	public class DragDropManager
	{
		private Panel Host { get; set; }
		private Point MousePosition { get; set; }
		private UIElement DragCursor { get; set; }	
		private bool HasProcessedEndDrag { get; set; }
		private IDroppable Item { get; set; }	

        /// <summary>
        /// Initializes a new instance of the DragDropManager class.
        /// </summary>
        /// <param name="host">The hosting panel where the drag cursor instance will be added.</param>
		public DragDropManager(Panel host)
		{
			this.Host = host;
			this.Host.LostFocus += new RoutedEventHandler(OnHostLostFocus);
		}

		private void OnHostLostFocus(object sender, RoutedEventArgs e)
		{
			EndDrag(false);
		}

		private void OnDragCursorLostMouseCapture(object sender, MouseEventArgs e)
		{
			EndDrag(false);
		}

		private void OnDragCursorLostFocus(object sender, RoutedEventArgs e)
		{
			EndDrag(false);
		}

		private void OnDragCursorMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).ReleaseMouseCapture();

			this.MousePosition = e.GetPosition(null);

			this.EndDrag(true);
		}

		private void OnDragCursorMouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(null);

			double deltaX = position.X - this.MousePosition.X;
			double deltaY = position.Y - this.MousePosition.Y;

			Point newPosition = new Point(
				((double)this.DragCursor.GetValue(Canvas.LeftProperty)) + deltaX,
				((double)this.DragCursor.GetValue(Canvas.TopProperty)) + deltaY);

			this.DragCursor.SetValue(Canvas.LeftProperty, newPosition.X);
			this.DragCursor.SetValue(Canvas.TopProperty, newPosition.Y);

			this.MousePosition = position;
		}

        /// <summary>
        /// Begins a drag operation for the specified IDroppable instance starting at the specified mousePosition.
        /// </summary>
        /// <param name="droppable">The IDroppable instance to being dragging.</param>
        /// <param name="mousePosition">The Point at which the drag operation will begin.</param>
		public void BeginDrag(IDroppable droppable, Point mousePosition)
		{
			if (droppable != null && this.Host != null)
			{
				Logger.LogDebug("Begin Drag: X={0}, Y={1}", mousePosition.X, mousePosition.Y);
				this.Item = droppable;

				this.DragCursor = droppable.GetDragCursor();
				this.DragCursor.LostFocus += new RoutedEventHandler(OnDragCursorLostFocus);
				this.DragCursor.LostMouseCapture += new MouseEventHandler(OnDragCursorLostMouseCapture);

				// Add the drag cursor to the host control and set the z-index very high.
				this.DragCursor.SetValue(Canvas.ZIndexProperty, 5000);
				this.Host.Children.Add(this.DragCursor);
				this.Host.Cursor = Cursors.Hand;

				this.MousePosition = mousePosition;
				this.DragCursor.SetValue(Canvas.LeftProperty, this.MousePosition.X);
				this.DragCursor.SetValue(Canvas.TopProperty, this.MousePosition.Y);
				this.DragCursor.Visibility = Visibility.Visible;

				this.HasProcessedEndDrag = false;

				this.DragCursor.CaptureMouse();

				this.DragCursor.MouseLeftButtonUp += new MouseButtonEventHandler(OnDragCursorMouseLeftButtonUp);
				this.DragCursor.MouseMove += new MouseEventHandler(OnDragCursorMouseMove);
			}
		}

		private void EndDrag(bool processDrop)
		{
			// This prevents the mouse up event or the end drag event firing more than once from causing the
			// events to bubble up and the drop be handled more than once.
			Logger.LogDebug("End Drag: X={0}, Y={1}", this.MousePosition.X, this.MousePosition.Y);
			if (this.HasProcessedEndDrag) return;
			this.HasProcessedEndDrag = true;

			// Reshow the cursor and hide the source image.
			this.Host.Cursor = Cursors.Arrow;

			// Hide the drag cursor.
			if (this.DragCursor != null)
			{
				this.DragCursor.Visibility = Visibility.Collapsed;

                this.DragCursor.LostFocus -= new RoutedEventHandler(OnDragCursorLostFocus);
                this.DragCursor.LostMouseCapture -= new MouseEventHandler(OnDragCursorLostMouseCapture);
				this.DragCursor.MouseLeftButtonUp -= new MouseButtonEventHandler(OnDragCursorMouseLeftButtonUp);
				this.DragCursor.MouseMove -= new MouseEventHandler(OnDragCursorMouseMove);
			}

			if (processDrop)
			{
				Logger.LogDebug("Process Drop...");
				// Search within host for an IDropContainer instance within the area of the current mouse position.
				var container = (from v in VisualTreeHelper.FindElementsInHostCoordinates(this.MousePosition, this.Host)
								where (v as IDropContainer) != null
								select v as IDropContainer).FirstOrDefault();
				if (container != null)
				{
					Logger.LogDebug("Drop...");
					container.Drop(this.Item);
				}
			}
		}
	}

    /// <summary>
    /// Provides a delegate for handling IDroppable.BeginDrag events.
    /// </summary>
    /// <param name="sender">The IDroppable initiating the drag operation.</param>
    /// <param name="e">Event data for the operation.</param>
	public delegate void BeginDragEventHandler(object sender, BeginDragEventArgs e);

    /// <summary>
    /// Represents event data for a BeginDrag event.
    /// </summary>
	public class BeginDragEventArgs : EventArgs
	{
        /// <summary>
        /// Gets or sets the IDroppable initiating the event.
        /// </summary>
		public IDroppable Droppable { get; set; }

        /// <summary>
        /// Gets or sets the Point at which the drag operation should begin.
        /// </summary>
		public Point MousePosition { get; set; }	
	}
}
