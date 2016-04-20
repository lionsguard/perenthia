using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Lionsguard.UI.WebControls
{
	[ToolboxData("<{0}:SharedHeader runat=server></{0}:SharedHeader>")]
	public class SharedHeader : WebControl
	{
		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("/")]
		[Localizable(true)]
		public string LoginReturnUrl
		{
			get
			{
				String s = (String)ViewState["LoginReturnUrl"];
				return ((s == null) ? String.Empty : s);
			}
			set { ViewState["LoginReturnUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("/")]
		[Localizable(true)]
		public string LogoutReturnUrl
		{
			get
			{
				String s = (String)ViewState["LogoutReturnUrl"];
				return ((s == null) ? String.Empty : s);
			}
			set { ViewState["LogoutReturnUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("/")]
		[Localizable(true)]
		public string SignupReturnUrl
		{
			get
			{
				String s = (String)ViewState["SignupReturnUrl"];
				return ((s == null) ? String.Empty : s);
			}
			set { ViewState["SignupReturnUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(SharedHeaderTheme.WhiteAndBlack)]
		[Localizable(true)]
		public SharedHeaderTheme Theme
		{
			get
			{
				object obj = ViewState["Theme"];
				if (obj != null)
				{
					return (SharedHeaderTheme)obj;
				}
				return SharedHeaderTheme.WhiteAndBlack;
			}
			set { ViewState["Theme"] = value; }
		}

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("/")]
		[Localizable(true)]
		public string AuthReturnUrl
		{
			get
			{
				String s = (String)ViewState["AuthReturnUrl"];
				return ((s == null) ? String.Empty : s);
			}
			set { ViewState["AuthReturnUrl"] = value; }
		}


		protected override void RenderContents(HtmlTextWriter writer)
		{
			////<script type="text/javascript">
			////    var lg_theme = '1';
			////    var lg_login_return_url = '/';
			////    var lg_logout_return_url = '/';
			////    var lg_signup_return_url = '/';
			////</script>
			////<script type="text/javascript" src="http://<server>/SharedHeader.axd"></script>

			//StringBuilder sb = new StringBuilder();
			////sb.Append("<script type=\"text/javascript\">").Append(Environment.NewLine);
			////sb.AppendFormat("var lg_theme = '{0}';", (int)this.Theme).Append(Environment.NewLine);
			////sb.AppendFormat("var lg_login_return_url = '{0}';", this.LoginReturnUrl).Append(Environment.NewLine);
			////sb.AppendFormat("var lg_logout_return_url = '{0}';", this.LogoutReturnUrl).Append(Environment.NewLine);
			////sb.AppendFormat("var lg_signup_return_url = '{0}';", this.SignupReturnUrl).Append(Environment.NewLine);
			////sb.Append("</script>").Append(Environment.NewLine);

			//SecureQueryString qs = new SecureQueryString();
			//qs.Add("lg_theme", Convert.ToString((int)this.Theme));
			//qs.Add("lg_auth_return_url", this.AuthReturnUrl);
			//qs.Add("lg_login_return_url", this.LoginReturnUrl);
			//qs.Add("lg_logout_return_url", this.LogoutReturnUrl);
			//qs.Add("lg_signup_return_url", this.SignupReturnUrl);

			//sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}/SharedHeader.axd?{1}\"></script>", 
			//    Settings.RootUrl, 
			//    qs.ToString());

			//output.Write(sb.ToString());

			HtmlGenericControl divOuter = new HtmlGenericControl("div");
			HtmlGenericControl divImage = new HtmlGenericControl("div");
			HtmlGenericControl divLinks = new HtmlGenericControl("div");

			divOuter.Controls.Add(divImage);
			divOuter.Controls.Add(divLinks);

			divOuter.Style.Add(HtmlTextWriterStyle.Height, "22px");
			divOuter.Style.Add(HtmlTextWriterStyle.Padding, "2px");

			divImage.Style.Add("float", "left");
			divImage.Style.Add(HtmlTextWriterStyle.Width, "250px");
			divImage.Style.Add(HtmlTextWriterStyle.PaddingTop, "1px");
			HyperLink lnk = new HyperLink();
			lnk.Target = "_parent";
			lnk.NavigateUrl = Lionsguard.Settings.RootUrl;
			lnk.ImageUrl = String.Format("{0}/common/shared/shared-header-{1}.gif", Lionsguard.Settings.RootUrl, (int)this.Theme);
			lnk.ToolTip = "Lionsguard Technologies, LLC";
			divImage.Controls.Add(lnk);

			divLinks.Style.Add("float", "right");
			divLinks.Style.Add(HtmlTextWriterStyle.FontSize, "10px");
			divLinks.Style.Add(HtmlTextWriterStyle.PaddingTop, "3px");
			divLinks.Style.Add(HtmlTextWriterStyle.PaddingRight, "4px");

			if (!String.IsNullOrEmpty(this.Context.User.Identity.Name))
			{
				divLinks.Controls.Add(this.CreateLink(String.Format("{0}?{1}", Lionsguard.Settings.MyAccountUrl, this.CreateSecureQueryString(LoginReturnUrl)), "My Lionsguard Account"));
				divLinks.Controls.Add(this.CreateLinkSpacer());
				divLinks.Controls.Add(this.CreateLink(String.Format("{0}?{1}", Lionsguard.Settings.EmblemUrl, this.CreateSecureQueryString(LoginReturnUrl)), "Purchase Emblem"));
				divLinks.Controls.Add(this.CreateLinkSpacer());
				divLinks.Controls.Add(this.CreateLink(String.Format("{0}?{1}", Lionsguard.Settings.ChangePasswordUrl, this.CreateSecureQueryString(LoginReturnUrl)), "Change Password"));
				divLinks.Controls.Add(this.CreateLinkSpacer());
				divLinks.Controls.Add(this.CreateLink(String.Format("{0}?{1}", Lionsguard.Settings.LogoutUrl, this.CreateSecureQueryString(LogoutReturnUrl)), "Logout"));
				divLinks.Controls.Add(new LiteralControl(String.Format("&nbsp;&nbsp;Not {0}? ", this.Context.User.Identity.Name)));
				divLinks.Controls.Add(this.CreateLink(String.Format("{0}?{1}", Lionsguard.Settings.LogoutUrl, this.CreateSecureQueryString(LogoutReturnUrl)), "click here"));
			}
			else
			{
				divLinks.Controls.Add(this.CreateLink(String.Format("{0}?{1}", Lionsguard.Settings.LoginUrl, this.CreateSecureQueryString(LoginReturnUrl)), "Login"));
				divLinks.Controls.Add(new LiteralControl("&nbsp;&nbsp;or&nbsp;&nbsp;"));
				divLinks.Controls.Add(this.CreateLink(String.Format("{0}?{1}", Lionsguard.Settings.SignUpUrl, this.CreateSecureQueryString(SignupReturnUrl)), "Create New Account"));
			}

			divOuter.RenderControl(writer);
		}

		private HyperLink CreateLink(string url, string text)
		{
			HyperLink lnk = new HyperLink();
			lnk.Target = "_parent";
			lnk.NavigateUrl = url;
			lnk.Text = text;
			return lnk;
		}

		private LiteralControl CreateLinkSpacer()
		{
			return new LiteralControl("&nbsp;&nbsp;|&nbsp;&nbsp;");
		}

		private string CreateSecureQueryString(string returnUrl)
		{
			SecureQueryString qs = new SecureQueryString();
			qs.Add("AuthReturnUrl", this.AuthReturnUrl);
			qs.Add("ReturnUrl", returnUrl);
			return qs.ToString();
		}
	}

	public enum SharedHeaderTheme
	{
		WhiteAndBlack = 0,
		BlackAndGold = 1,
		ClearAndGold = 2,
	}
}
