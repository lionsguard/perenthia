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

namespace Lionsguard
{
    /// <summary>
    /// Defines an object that can be dragged and dropped via the DragDropManager.
    /// </summary>
	public interface IDroppable
	{
		/// <summary>
		/// An event that is raised when the IDroppable item begins the drag operation.
		/// </summary>
		event BeginDragEventHandler BeginDrag;

		/// <summary>
		/// Gets the UIElement that will server as the cursor replacement when dragging the current object.
		/// </summary>
		/// <returns></returns>
		UIElement GetDragCursor();
	}

    /// <summary>
    /// Defines an object that accept IDroppable items as part of a drop operation.
    /// </summary>
	public interface IDropContainer
	{
		/// <summary>
		/// Handles the drop of the specified IDroppable item onto the container.
		/// </summary>
		/// <param name="item"></param>
		void Drop(IDroppable item);
	}
}
