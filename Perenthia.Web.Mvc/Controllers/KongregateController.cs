using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Perenthia.Web.Models;

namespace Perenthia.Web.Controllers
{
    public class KongregateController : Controller
    {
		public ActionResult Index()
		{
			return Game();
		}
        public ActionResult Game()
		{
			var viewData = new PlayViewData();

			//SecurityServiceClient securityClient = new SecurityServiceClient();
			//string authKey = securityClient.GetUserAuthKey(Lionsguard.Security.SecurityManager.Encrypt(this.User.Identity.Name));

			//ArmorialServiceClient armorialClient = new ArmorialServiceClient();
			//string version = armorialClient.GetGameVersion();

			// authKey={0},servicesRootUri={1},gameService={2},armorialService={3},builderService={4},mediaUri={5},version={6},mode={7}
			viewData.AuthKey = String.Empty;//authKey;
			viewData.ServicesRootUri = String.Concat(WebUtils.GetMembersUri(), "Services");
			viewData.GameService = "HttpGameServer.ashx";
			viewData.ArmorialService = "ArmorialService.svc";
			viewData.BuilderService = "BuilderService.svc";
			viewData.SecurityService = "SecurityService.svc";
			viewData.MediaUri = String.Concat(Lionsguard.Util.GetServerUrl(this.ControllerContext.HttpContext.Request.Url), "/common/media");
			viewData.Version = "1.0.12";//version;
			viewData.Mode = "play";

			return View(viewData);
        }

    }
}
