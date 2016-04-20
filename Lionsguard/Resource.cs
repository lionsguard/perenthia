using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;

namespace Lionsguard
{
	public static class Resource
	{
		public static string GetResource(string path, params object[] args)
		{
			string content;
			using (StreamReader reader = new StreamReader(path))
			{
				content = reader.ReadToEnd();
			}
			if (args != null)
			{
				return String.Format(content, args);
			}
			return content;
		}

        internal static string GetLocalResource(string name, params object[] args)
        {
            string content;
            using (StreamReader reader = new StreamReader(Assembly.GetAssembly(typeof(Resource)).GetManifestResourceStream(name)))
			{
				content = reader.ReadToEnd();
            }
            if (args != null)
            {
                return String.Format(content, args);
            }
            return content;
        }
	}
}
