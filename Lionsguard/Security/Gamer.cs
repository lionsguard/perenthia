using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lionsguard.Security
{
	public class Gamer
	{
		public string UserName { get; set; }
		public string UserToken { get; set; }
		public bool IsAuthenticated { get; set; }	
	}
}
