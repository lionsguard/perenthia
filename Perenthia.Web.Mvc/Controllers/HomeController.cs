using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using System.Xml;

using RPXLib;

using Lionsguard.Content;
using Lionsguard.Controllers;
using Lionsguard.Forums;
using Lionsguard.Security;

using Perenthia.Web.Models;
using Radiance.Contract;
using Perenthia.Web.Services.Security;
using System.ServiceModel;
using Perenthia.Web.ActionResults;

namespace Perenthia.Web.Controllers
{
	public class HomeController : Lionsguard.Controllers.ControllerBase
	{
		public HomeController()
		{
        }

		public ActionResult Index()
		{
            SyndicationFeed blog = this.GetSyndicationFeed("http://blog.perenthia.com/syndication.axd");
            if (blog != null)
            {
				var items = blog.Items.Take(10);
                ViewData["BlogFeedItems"] = items;
            }

            try
            {
                ViewData["ForumTopics"] = ForumManager.GetTopTopics(1, 4);
            }
#if DEBUG
			catch (Exception) { throw; }
#else
            catch (Exception ex)
            {
                Lionsguard.Log.Write(ex.ToString(), true);
            }
#endif

            try
            {
				var client = ClientFactory.CreateClient<IArmorialService>(WebUtils.ArmorialServiceUri);
                ViewData["FeaturedCharacter"] = client.GetFeaturedCharacter();
            }
            catch (System.ServiceModel.CommunicationObjectFaultedException) { }
#if DEBUG
			catch (Exception) { throw; }
#else
            catch (Exception ex)
            {
                Lionsguard.Log.Write(ex.ToString(), true);
            }
#endif

			return View();
		}

        private SyndicationFeed GetSyndicationFeed(string feedUri)
        {
            try
            {
                XmlReader reader = XmlReader.Create(feedUri);
                return SyndicationFeed.Load(reader);
            }
#if DEBUG
			catch (Exception) { throw; }
#else
            catch (Exception ex)
            {
                Lionsguard.Log.Write(ex.ToString(), true);
            }
#endif
            return null;
        }

        public ActionResult AuthVerify()
        {
			RpxViewData viewData = new RpxViewData();
			string token = this.Request.Params["token"];
			if (!String.IsNullOrEmpty(token))
			{
				var rpx = new RPXService(WebUtils.GetRpxSettings());

				var auth = rpx.GetAuthenticationDetails(token, true);
				if (auth != null)
				{
					viewData.RpxIndentifier = auth.Identifier;
					if (!String.IsNullOrEmpty(auth.LocalKey))
					{
						// Verify against DB...
						User user = SecurityManager.GetUser(auth.LocalKey);
						if (user != null)
						{
							//Game.SetAuthCookie(auth.LocalKey, true);
							SecurityManager.SignIn(auth.LocalKey, true);
							return Redirect("/");
						}
						else
						{
							return Redirect("/Login");
						}
					}
					else
					{
						// Requires Mapping.
						// Do Nothing...
					}
				}
				else
				{
					// Auth Failed.
					return Redirect("/Login");
				}
			}
			else
			{
				return Redirect("/Login");
			}

            return View(viewData);
        }

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AuthVerify(string username, string password, string rpxId)
		{
			RpxViewData viewData = new RpxViewData();
			viewData.RpxIndentifier = rpxId;

			bool isApproved = false;
			if (SecurityManager.ValidateUser(username, password, out isApproved))
			{
				if (!isApproved)
				{
					return RedirectToAction("Confirm", "Security");
				}
				else
				{
					var rpx = new RPXService(WebUtils.GetRpxSettings());
					rpx.MapLocalKey(viewData.RpxIndentifier, username);
					SecurityManager.SignIn(username, true);
					RedirectToAction("Index", "Home");
				}
			}

			return View(viewData);
		}

		public ActionResult Image()
		{
			ImageViewData viewData = new ImageViewData();
			viewData.Title = this.GetRouteValue<string>("title", Request.Params["title"]);
			viewData.ImageUrl = this.GetRouteValue<string>("image", Request.Params["image"]);
			return View(viewData);
		}

		public ActionResult News()
		{
			FeedViewData viewData = new FeedViewData();

			try
			{
				SyndicationFeed blog = this.GetSyndicationFeed("http://blog.perenthia.com/syndication.axd");
				if (blog != null)
				{
					viewData.Items = blog.Items.Take(8);
				}
			}
			catch (Exception) { }

			return View(viewData);
		}

		public ActionResult Press()
		{
			return RedirectToAction("News");
		}

		//[Authorize]
		public ActionResult Play()
		{
			var useSilverlight = !Request.Browser.IsMobileDevice;// || Request.Headers.AllKeys.Contains("x_skyfire_phone header");

			if (!useSilverlight)
			{
				// Return the AJAX enabled client page.
				return RedirectToAction("PlayAjax");
			}

			// Otherwise return the Silverlight client page.
			return View();
		}

		public ActionResult PlayAjax()
		{
			return View();
		}

		public ActionResult SiteMap()
		{
			return View();
		}

		public ActionResult GettingStarted()
		{
			return View();
		}

		public ActionResult Version()
		{
			return new VersionActionResult();
		}
	}
}
