using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Perenthia.Web.Models
{
	public class FeedViewData
	{
		public IEnumerable<SyndicationItem> Items { get; set; }	
	}
}
