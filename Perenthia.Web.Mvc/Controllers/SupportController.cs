using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Perenthia.Web.Controllers
{
	public class SupportController : Lionsguard.Controllers.ControllerBase
    {
        //
        // GET: /Support/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Faq()
		{
			return View();
		}
    }
}
