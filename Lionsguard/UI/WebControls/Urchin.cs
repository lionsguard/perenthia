using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lionsguard.UI.WebControls
{
	[ToolboxData("<{0}:Urchin runat=server></{0}:Urchin>")]
	public class Urchin : WebControl
	{
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
		}
		public override void RenderEndTag(HtmlTextWriter writer)
		{			
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write("urchin");
				return;
			}
			
			if (!this.Page.Request.Url.Host.Equals("localhost"))
			{
				// Urchin script reference.
				writer.AddAttribute(HtmlTextWriterAttribute.Src, "http://www.google-analytics.com/urchin.js");
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
				writer.RenderBeginTag(HtmlTextWriterTag.Script);
				writer.RenderEndTag();

				// Urchin and silverlight call.
				writer.AddAttribute(HtmlTextWriterAttribute.Src, this.ScriptUrl);
				writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
				writer.RenderBeginTag(HtmlTextWriterTag.Script);
				writer.RenderEndTag();

				writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
				writer.RenderBeginTag(HtmlTextWriterTag.Script);
				writer.Write("lionsguard_urchin_track();");
				writer.RenderEndTag();

			}
		}

		public string ScriptUrl
		{
			get
			{
				object obj = ViewState["ScriptUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return "http://www.lionsguard.com/common/scripts/urchin.js";
			}
			set { ViewState["ScriptUrl"] = value; }
		}
	}
}
