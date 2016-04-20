using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Text;
using System.Web.Routing;

using Lionsguard.Mvc;
using Lionsguard.Models;

namespace Lionsguard.Controllers
{
	public abstract class ControllerBase : Controller
	{
		protected override void OnException(ExceptionContext filterContext)
		{
			this.WriteLog(filterContext.Exception, filterContext.HttpContext.Request);

			string viewName = "Error";
			int statusCode = 500;
			if (filterContext.Exception is System.IO.FileNotFoundException)
			{
				viewName = "NotFound";
				statusCode = 404;
			}

			var info = new ErrorInfoViewData(filterContext.Exception,
				this.GetRouteValue<string>("controller", "Home"),
				this.GetRouteValue<string>("action", "Index"));

#if DEBUG
			info.DisplayErrorDetails = true;
#endif

			var result = new ViewResult();
			result.ViewName = viewName;
			result.ViewData = new ViewDataDictionary<ErrorInfoViewData>(info);
			result.TempData = filterContext.Controller.TempData;

			filterContext.Result = result;
			filterContext.ExceptionHandled = true;
			filterContext.HttpContext.Response.Clear(); 
			filterContext.HttpContext.Response.StatusCode = statusCode;
			filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
		}

		public ActionResult Error()
		{
			var e = Request.Params["e"];
			if (!String.IsNullOrEmpty(e))
			{
				this.WriteLog(e, Request);
			}

			return View();
		}

		public ActionResult NotFound()
		{
			return View();
		}

		protected internal void WriteLog(Exception exception, HttpRequestBase request)
		{
			this.WriteLog(exception.ToString(), request);
		}

		protected internal void WriteLog(string exception, HttpRequestBase request)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(DateTime.Now.ToString());
			sb.AppendLine();
			sb.AppendLine(exception);
			sb.AppendLine();
			sb.AppendLine("FORM VALUES:");
			foreach (var item in request.Form.AllKeys)
			{
				sb.AppendFormat("{0} = {1}", item, request.Form[item]).AppendLine();
			}
			sb.AppendLine("QUERYSTRING VALUES:");
			foreach (var item in request.QueryString.AllKeys)
			{
				sb.AppendFormat("{0} = {1}", item, request.QueryString[item]).AppendLine();
			}
			sb.AppendLine();

#if DEBUG
			System.Diagnostics.Debug.WriteLine(sb.ToString());
#else
			Lionsguard.Log.Write(sb.ToString(), true);
#endif
		}

		protected T GetRouteValue<T>(string key, T defaultValue)
		{
			return this.RouteData.Values.Value<T>(key, defaultValue);
		}
	}
}
