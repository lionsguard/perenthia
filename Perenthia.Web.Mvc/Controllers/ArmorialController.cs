using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Perenthia.Web.ActionResults;
using Perenthia.Web.Models;
using MvcSiteMap.Core;
using Radiance.Contract;
using System.Text;

namespace Perenthia.Web.Controllers
{
	public class ArmorialController : Lionsguard.Controllers.ControllerBase
	{
		//
		// GET: /Armorial/

		public ActionResult Index()
		{
			return View("Search");
		}

		[ValidateInput(false)]
		public ActionResult Signatures()
		{
			var name = Request.Params["name"] ?? String.Empty;
			var type = Request.Params["type"] ?? String.Empty;

			var sb = new StringBuilder();

			if (!String.IsNullOrEmpty(name))
			{
				sb.AppendFormat("<a href=\"http://wwww.perenthia.com/Armorial/Character/{0}/{1}\" title=\"{0} Character Signature\">", name, type);
				sb.AppendFormat("<img src=\"http://wwww.perenthia.com/Armorial/Character/{0}/{1}\" alt=\"{0} Character Signature\" />", name, type);
				sb.Append("</a>");
			}

			ViewData["code"] = sb.ToString();

			return View();
		}

		private int MaxRows = 10;
		public ActionResult Search()
		{
			ArmorialViewData viewData = new ArmorialViewData();

			viewData.Query = Request.Params["query"] ?? String.Empty;
			if (String.IsNullOrEmpty(viewData.Query))
				viewData.Query = this.GetRouteValue<string>("query", String.Empty);
			viewData.SearchType = this.GetRouteValue<string>("searchType", "all");
			viewData.SortBy = this.GetRouteValue<string>("sortBy", String.Empty);
			viewData.SortDirection = this.GetRouteValue<string>("sortDirection", String.Empty);
			viewData.Tab = this.GetRouteValue<ArmorialTab>("tab", ArmorialTab.Characters);
			viewData.PageIndex = this.GetRouteValue<int>("start", 0);
			viewData.MaxRows = MaxRows;

			if (!String.IsNullOrEmpty(viewData.Query))
									//|| (!String.IsNullOrEmpty(viewData.SearchType)
									//    && !String.IsNullOrEmpty(viewData.SortBy)
									//    && !String.IsNullOrEmpty(viewData.SortDirection)))
			{
				try
				{
					// Execute Query
					var client = ClientFactory.CreateClient<IArmorialService>(WebUtils.ArmorialServiceUri); //new Perenthia.Web.Services.Search.SearchServiceClient();
					var results = client.Search(viewData.Query, viewData.SearchType, viewData.SortBy, viewData.SortDirection, viewData.PageIndex * MaxRows, MaxRows);
					if (results != null && results.Data != null)
					{
						if (results.PageCounts.ContainsKey(viewData.Tab.ToString()))
						{
							viewData.TotalRowCount = results.PageCounts[viewData.Tab.ToString()];
						}
						switch (viewData.Tab)
						{
							case ArmorialTab.Characters:
								viewData.Results = results.Data.Cast<AvatarData>();
								break;
							case ArmorialTab.Households:
								viewData.Results = results.Data.Cast<HouseholdData>();
								break;
							case ArmorialTab.Items:
								viewData.Results = results.Data.Cast<ItemData>();
								break;
							case ArmorialTab.Creatures:
								viewData.Results = results.Data.Cast<AvatarData>();
								break;
						}
					}
				}
				catch (Exception ex)
				{
#if DEBUG
					throw ex;
#else
					Lionsguard.Log.Write(ex.ToString(), true);
#endif
				}
			}

			return View("Search", viewData);
		}

		public ActionResult Character()
		{
			// Args:
			// name = Signature Panel
			// detail = Details Page
			// xml = XML Details Data
			// html = HTML Signature Panel
			// xl = XL signature image

			string name = this.GetRouteValue<string>("name", String.Empty);
			if (String.IsNullOrEmpty(name))
			{
				return RedirectToAction("Search");
			}

			CharacterType type = this.GetRouteValue<CharacterType>("type", CharacterType.None);

			var client = ClientFactory.CreateClient<IArmorialService>(WebUtils.ArmorialServiceUri);
			var player = client.FindCharacter(name);
			switch (type)
			{
				case CharacterType.None:
				case CharacterType.XL:
					// Character Signature Image
					return new ArmorialAvatarImageResult(player, type == CharacterType.XL ? "xl" : String.Empty);
				case CharacterType.Detail:
					// Character Detail Page
					// The default view services this request.
					break;
				case CharacterType.Xml:
					// Character Detail Xml
					break;
				case CharacterType.Html:
					// Character Signature Html
					break;
			}

			// Site Map Node
			SiteMap.CurrentNode.Title = player.Name;

			// TODO: If link was from a search results screen then change the parent node to be search...

			return View("Character", player);
		}

		public ActionResult Household()
		{
			string name = this.GetRouteValue<string>("name", String.Empty);
			if (String.IsNullOrEmpty(name))
			{
				return RedirectToAction("Search");
			}

			var household = new HouseholdData();
			return View("Household", household);
		}

		public ActionResult Item()
		{
			string name = this.GetRouteValue<string>("name", String.Empty);
			if (String.IsNullOrEmpty(name))
			{
				return RedirectToAction("Search");
			}

			var item = new ItemData();
			return View("Item", item);
		}

		public ActionResult Creature()
		{
			string name = this.GetRouteValue<string>("name", String.Empty);
			if (String.IsNullOrEmpty(name))
			{
				return RedirectToAction("Search");
			}

			var creature = new AvatarData();
			return View("Creature", creature);
		}
	}
}
