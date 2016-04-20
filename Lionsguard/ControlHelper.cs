using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Lionsguard
{
	/// <summary>
	/// Provides extension methods for the System.Web.UI.Control class.
	/// </summary>
	public static class ControlHelper
	{
		/// <summary>
		/// Searches the specified naming container non-recursively for a server control with the specified id parameter.
		/// </summary>
		/// <param name="container">The control to start searching.</param>
		/// <param name="id">The identifier for the control to be found.</param>
		/// <returns>The specified control, or null if the specified control does not exist.</returns>
		public static Control FindControl(Control container, string id)
		{
			Control control = container.FindControl(id);
			if (control != null) return control;

			Stack<Control> stack = new Stack<Control>();
			for (int i = 0; i < control.Controls.Count; i++)
			{
				stack.Push(control.Controls[i]);
			}

			while (stack.Count > 0)
			{
				control = stack.Pop();
				Control ctl = control.FindControl(id);
				if (ctl != null)
				{
					return ctl;
				}
				else
				{
					if (control.HasControls())
					{
						for (int i = 0; i < control.Controls.Count; i++)
						{
							stack.Push(control.Controls[i]);
						}
					}
				}
			}
			return null;
		}
	}
}
