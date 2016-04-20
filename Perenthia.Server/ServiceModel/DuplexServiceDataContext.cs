using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace Perenthia.ServiceModel
{
	public class DuplexServiceDataContext : TableServiceContext
	{
		public DuplexServiceDataContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }
	}
}
