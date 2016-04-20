using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Radiance.Contract;

namespace Perenthia.Web.Controllers
{
	public class LoreController : Lionsguard.Controllers.ControllerBase
    {
        //
        // GET: /Lore/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult Races()
        {
			ViewData["Races"] = Game.Races;

			string name = Request.QueryString["name"];
			string gender = Request.QueryString["gender"];

			var race = Game.Races.Where(r => r.Name == name).FirstOrDefault();
			if (race != null)
			{
				ViewData["Race"] = race;
				ViewData["Gender"] = gender;
			}

            return View();
        }

        public ActionResult AttributesAndSkills()
        {
			ViewData["Skills"] = Game.Skills;
            return View();
        }

        public ActionResult Development()
        {
            return View();
        }

		public ActionResult Households()
		{
			return View();
		}
    }

	public class RaceInfo
	{
		public RaceData Race { get; set; }
		public string Gender { get; set; }	
	}
}
