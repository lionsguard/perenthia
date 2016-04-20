using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Perenthia.Web.Models;
using Lionsguard.Payments;

namespace Perenthia.Web.Controllers
{
    public class PaymentsController : Controller
    {
        public ActionResult Index()
        {
			var userid = "calbert";

			var parameters = new Dictionary<string, string>();
			parameters.Add("format", "iframe");
			parameters.Add("layout", "1");
			parameters.Add("user_id", userid);

			var ts = Jambool.GetTimestamp();
			parameters.Add("ts", ts.ToString());

			var sig = Jambool.CreateSignature(parameters.ToArray());

			var viewData = new PaymentViewData();
			viewData.OfferId = Jambool.OfferId;
			viewData.Parameters = String.Format("format=iframe&layout=1&ts={0}&user_id={1}&sig={2}", ts, userid, sig);
            return View(viewData);
        }

		public ActionResult Error()
		{
			return View();
		}

		public ActionResult Complete()
		{
			return View();
		}

		public ActionResult Postback()
		{
			return View();
		}
    }
}
