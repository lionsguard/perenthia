using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lionsguard.Mvc;

namespace Perenthia.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			//=====================================================================================
			// Security Routes
			//=====================================================================================
			routes.MapRoute("Login",
				"Login",
				new { controller = "Security", action = "Login" });

			routes.MapRoute("Logout",
				"Logout",
				new { controller = "Security", action = "Logout" });

			routes.MapRoute("SignUp",
				"SignUp",
				new { controller = "Security", action = "SignUp" });

			routes.MapRoute("ForgotPassword",
				"ForgotPassword",
				new { controller = "Security", action = "ForgotPassword" });

			routes.MapRoute("ChangePassword",
				"ChangePassword",
				new { controller = "Security", action = "ChangePassword" });

			//=====================================================================================
			// Armorial Routes
			//=====================================================================================
			routes.MapRoute("Character",
				"Armorial/Character/{name}/{type}",
				new { controller = "Armorial", action = "Character", name = "", type = "" });

			routes.MapRoute("Household",
				"Armorial/Household/{name}",
				new { controller = "Armorial", action = "Household", name = "" });

			routes.MapRoute("Item",
				"Armorial/Item/{name}",
				new { controller = "Armorial", action = "Item", name = "" });

			routes.MapRoute("Creature",
				"Armorial/Creature/{name}",
				new { controller = "Armorial", action = "Creature", name = "" });

			routes.MapRoute("Signatures",
				"Armorial/Signatures",
				new { controller = "Armorial", action = "Signatures" });

			routes.MapRoute("Armorial", 
				"Armorial/{searchType}/{sortBy}/{sortDirection}/{tab}/{start}/{query}",
				new { controller = "Armorial", action = "Search", searchType = "all", sortBy = "level", sortDirection = "desc", tab = "characters", start = 0, query = "" });

			//=====================================================================================
			// Image Routes
			//=====================================================================================
			routes.MapRoute("Image",
				"Image",
				new { controller = "Home", action = "Image" });

			//=====================================================================================
			// AuthVerify Routes
			//=====================================================================================
			routes.MapRoute("AuthVerify",
				"AuthVerify",
				new { controller = "Home", action = "AuthVerify" });

			//=====================================================================================
			// News Routes
			//=====================================================================================
			routes.MapRoute("News",
				"News",
				new { controller = "Home", action = "News" });

			//=====================================================================================
			// Play Routes
			//=====================================================================================
			routes.MapRoute("Play",
				"Play",
				new { controller = "Home", action = "Play" });

			//=====================================================================================
			// SiteMap Route
			//=====================================================================================
			routes.MapRoute("SiteMap",
				"SiteMap",
				new { controller = "Home", action = "SiteMap" });

			//=====================================================================================
			// Error Routes
			//=====================================================================================
			routes.MapRoute("Error",
				"Error",
				new { controller = "Home", action = "Error" });

			routes.MapRoute("NotFound",
				"NotFound",
				new { controller = "Home", action = "NotFound" });

			//=====================================================================================
			// Misc Routes
			//=====================================================================================
			routes.MapRoute("Version",
				"Version",
				new { controller = "Home", action = "Version" });
			routes.MapRoute("GettingStarted",
				"GettingStarted",
				new { controller = "Home", action = "GettingStarted" });
			
			//=====================================================================================
			// Default Route
			//=====================================================================================
			routes.MapRoute(
				"Default",                                              // Route name
				"{controller}/{action}/{id}",                           // URL with parameters
				new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
			);

		}

		protected void Application_Start()
		{
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new MobileCapableWebFormViewEngine());

			RegisterRoutes(RouteTable.Routes);
		}
	}
}