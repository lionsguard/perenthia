using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Perenthia.Web.Controllers
{
	public class MediaController : Lionsguard.Controllers.ControllerBase
    {
        //
        // GET: /Media/

        public ActionResult Index()
        {
			return View("Screenshots");
		}

		public ActionResult Screenshots()
		{
			return View();
		}

    }
}
