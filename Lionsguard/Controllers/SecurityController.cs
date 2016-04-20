using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

using Lionsguard.Security;
using Lionsguard.Models;

namespace Lionsguard.Controllers
{
    [HandleError]
    public class SecurityController : ControllerBase
    {

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.

        public SecurityController()
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
		{
			string msg = this.GetRouteValue<string>("Message", null);
			if (String.IsNullOrEmpty(msg)) msg = Request.Params["Message"];
			ViewData["Message"] = msg;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult Login(string userName, string password, bool rememberMe, string returnUrl)
		{
			string msg = this.GetRouteValue<string>("Message", null);
			if (String.IsNullOrEmpty(msg)) msg = Request.Params["Message"];
			ViewData["Message"] = msg;

            bool isApproved = false;
            if (!ValidateLogin(userName, password, out isApproved))
            {
                return View();
            }

            if (!isApproved)
            {
                return RedirectToAction("Confirm");
            }

            SecurityManager.SignIn(userName, rememberMe);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {

			SecurityManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignUp()
        {

			ViewData["PasswordLength"] = SecurityManager.MinPasswordLength;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SignUp(string userName, string displayName, string email, string password, string confirmPassword, string birthDate, string securityQuestion, string securityAnswer)
        {

			ViewData["PasswordLength"] = SecurityManager.MinPasswordLength;

            if (ValidateRegistration(userName, displayName, email, password, confirmPassword, birthDate, securityQuestion, securityAnswer))
            {
                // Attempt to register the user
				MembershipCreateStatus createStatus = SecurityManager.CreateUser(userName, displayName, password, email, birthDate, securityQuestion, securityAnswer);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("Confirm", "Security");
                }
                else
                {
					ModelState.AddModelError("_FORM", SecurityManager.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult Confirm()
        {
            string code = Request.Params["code"];
			string email = Request.Params["email"];
            if (!String.IsNullOrEmpty(code) && !String.IsNullOrEmpty(email))
            {
                return this.Confirm(email, code);
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult Confirm(string email, string code)
        {
			if (String.IsNullOrEmpty(code) && String.IsNullOrEmpty(email))
			{
				return View();
			}

			if (SecurityManager.ConfirmUser(HttpUtility.UrlDecode(email), HttpUtility.UrlDecode(code)))
            {
                return RedirectToAction("Login", new { Message = "Your account has been activated!" } );
            }
            else
            {
                ModelState.AddModelError("_FORM", "The specified confirmation code could not be verified. Please check your entry and try again. If the problem persists, please contact customer support.");
            }
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForgotPassword(string email, string username)
        {
            // Validate the user by email or username, retrieve the security question and add it to the view data.
			string question = SecurityManager.GetSecurityQuestion(ref username, email);
            if (!String.IsNullOrEmpty(question))
            {
				return RedirectToAction("SecurityAnswer", new { username = username, question = HttpUtility.UrlEncode(question) });
            }
            else
            {
				ModelState.AddModelError("_FORM", "A Lionsguard ID for the specified username or email address could not be found. Please verify your entry and try again. If the problem persists, please contact customer support.");
				return View();
            }
		}

		public ActionResult SecurityAnswer(string username, string question)
		{
			SecurityAnswerViewData viewData = new SecurityAnswerViewData();
			viewData.SecurityQuestion = question;
			viewData.UserName = username;

			return View(viewData);
		}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResendPassword(string username, string securityQuestion, string securityAnswer)
        {
			if (SecurityManager.ResendPassword(username, securityAnswer))
            {
				return RedirectToAction("Login", new { Message = "Your Lionsguard ID account details have been sent to the email address associated with your account." });
            }
            
			ModelState.AddModelError("_FORM", "The answer supplied to the security question is invalid. Please verify your entry and try again. If the problem persists, please contact customer support.");

			return RedirectToAction("SecurityAnswer", new { username = username, question = securityQuestion });
        }

        [Authorize]
        public ActionResult ChangePassword()
        {

			ViewData["PasswordLength"] = SecurityManager.MinPasswordLength;

            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

			ViewData["PasswordLength"] = SecurityManager.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
				if (SecurityManager.ChangePassword(User.Identity.Name, currentPassword, newPassword))
                {
					SecurityManager.SignOut();
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
			if (newPassword == null || newPassword.Length < SecurityManager.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
						 SecurityManager.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogin(string userName, string password, out bool isApproved)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
			if (!SecurityManager.ValidateUser(userName, password, out isApproved))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegistration(string userName, string displayName, string email, string password, string confirmPassword, string birthDate, string securityQuestion, string securityAnswer)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (String.IsNullOrEmpty(securityQuestion))
            {
                ModelState.AddModelError("securityQuestion", "You must specify a security question you will be required to answer should you forget your password.");
            }
            if (String.IsNullOrEmpty(securityAnswer))
            {
                ModelState.AddModelError("securityAnswer", "You must specify an answer to the security question.");
            }
            DateTime dt;
            if (String.IsNullOrEmpty(birthDate) || !DateTime.TryParse(birthDate, out dt))
            {
                ModelState.AddModelError("birthDate", "You must specify a valid birth date.");
            }
			if (password == null || password.Length < SecurityManager.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
						 SecurityManager.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }
        #endregion
    }
}
