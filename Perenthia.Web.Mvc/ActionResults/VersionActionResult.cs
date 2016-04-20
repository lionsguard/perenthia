using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Perenthia.Web.ActionResults
{
	public class VersionActionResult : ActionResult
	{
		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = "text/plain";
			context.HttpContext.Response.Write(Game.GetVersion());
		}
	}
}
