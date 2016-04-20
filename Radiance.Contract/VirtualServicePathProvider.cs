using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web;
using System.ServiceModel.Activation;
using System.Web.Caching;
using System.Collections;

namespace Radiance.Contract
{
	public class VirtualServicePathProvider<TServiceHostFactory> : VirtualPathProvider where TServiceHostFactory : ServiceHostFactoryBase
	{
		private string _virtualDirectoryName;
		private string _serviceNamespace;

		public VirtualServicePathProvider(string virtualDirectoryName, string serviceNamespace)
		{
			_virtualDirectoryName = virtualDirectoryName;
			_serviceNamespace = serviceNamespace;
		}

		public override bool FileExists(string virtualPath)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

			if (IsVirtualFile(appRelativeVirtualPath))
			{
				return true;
			}
			else
			{
				return Previous.FileExists(virtualPath);
			}
		}

		public override System.Web.Hosting.VirtualFile GetFile(string virtualPath)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

			if (IsVirtualFile(appRelativeVirtualPath))
			{
				string srp = VirtualPathUtility.MakeRelative(_virtualDirectoryName + "/", virtualPath);
				string serviceClass = String.Concat(_serviceNamespace, ".", srp);
				if (serviceClass.EndsWith(".svc"))
				{
					serviceClass = serviceClass.Substring(0, serviceClass.LastIndexOf(".svc"));
				}
				return new VirtualServiceFile(virtualPath, serviceClass, typeof(TServiceHostFactory).FullName);
			}
			else
			{
				return Previous.GetFile(virtualPath);
			}
		}

		public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

			if (IsVirtualFile(appRelativeVirtualPath) || IsVirtualDirectory(appRelativeVirtualPath))
			{
				return null;
			}
			else
			{
				return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
			}
		}

		private bool IsVirtualFile(string appRelativeVirtualPath)
		{
			if (appRelativeVirtualPath.StartsWith(_virtualDirectoryName + "/", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private bool IsVirtualDirectory(string appRelativeVirtualPath)
		{
			return appRelativeVirtualPath.Equals(_virtualDirectoryName, StringComparison.OrdinalIgnoreCase);
		}

		private string ToAppRelativeVirtualPath(string virtualPath)
		{
			string appRelativeVirtualPath = VirtualPathUtility.ToAppRelative(virtualPath);

			if (!appRelativeVirtualPath.StartsWith("~/"))
			{
				throw new HttpException("Unexpectedly does not start with ~.");
			}
			return appRelativeVirtualPath;
		}
	}
}
