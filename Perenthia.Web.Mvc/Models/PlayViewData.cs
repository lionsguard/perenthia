using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perenthia.Web.Models
{
	public class PlayViewData
	{
		public string AuthKey { get; set; }
		public string ServicesRootUri { get; set; }
		public string GameService { get; set; }
		public string ArmorialService { get; set; }
		public string BuilderService { get; set; }
		public string SecurityService { get; set; }
		public string MediaUri { get; set; }
		public string Version { get; set; }
		public string Mode { get; set; }	
	}
}
