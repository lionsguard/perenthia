using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lionsguard.Models
{
	public class ErrorInfoViewData : System.Web.Mvc.HandleErrorInfo
	{
		public bool DisplayErrorDetails { get; set; }

		public ErrorInfoViewData(Exception exception, string controllerName, string actionName)
			: base(exception, controllerName, actionName)
		{
		}
	}
}
