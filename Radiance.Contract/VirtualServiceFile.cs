using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web.Hosting;
using System.Web;

namespace Radiance.Contract
{
	public class VirtualServiceFile : VirtualFile
	{
		private string _service;
		private string _factory;

		public VirtualServiceFile(string vp, string service, string factory)
			: base(vp)
		{
			_service = service;
			_factory = factory;
		}

		public override Stream Open()
		{
			MemoryStream ms = new MemoryStream();
			StreamWriter tw = new StreamWriter(ms);
			tw.Write(string.Format(CultureInfo.InvariantCulture,
			  "<%@ServiceHost language=c# Debug=\"true\" Service=\"{0}\" Factory=\"{1}\"%>",
			  HttpUtility.HtmlEncode(_service), HttpUtility.HtmlEncode(_factory)));
			tw.Flush();
			ms.Position = 0;
			return ms;
		}
	}
}
