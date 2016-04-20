using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Radiance
{
	public class HttpHelper
	{
		private HttpWebRequest Request { get; set; }
		public List<byte> PostData { get; private set; }
		public object State { get; set; }	

		public event HttpResponseFailedEventHandler ResponseFailed = delegate { };
		private void OnResponseFailed(HttpResponseFailedEventArgs e)
		{
			this.ResponseFailed(e);
		}

		public event HttpStreamEventHandler ResponseComplete = delegate { };
		private void OnResponseComplete(HttpStreamEventArgs e)
		{
			this.ResponseComplete(e);
		}

		public event HttpStreamEventHandler RequestBegin = delegate { };
		private void OnRequestBegin(HttpStreamEventArgs e)
		{
			this.RequestBegin(e);
		}

		public HttpHelper(Uri requestUri, string method)
		{
			this.Request = (HttpWebRequest)WebRequest.Create(requestUri);
			this.Request.ContentType = "application/x-www-form-urlencoded";
			this.Request.Method = method;
			this.PostData = new List<byte>();
		}

		public void Execute()
		{
			this.Request.BeginGetRequestStream(new AsyncCallback(HttpHelper.BeginRequest), this);
		}

		private static void BeginRequest(IAsyncResult ar)
		{
			HttpHelper helper = ar.AsyncState as HttpHelper;
			if (helper != null)
			{
				Stream stream = helper.Request.EndGetRequestStream(ar);
				if (stream != null)
				{
					// Raise the begin request event to allow writing to the stream before the post data.
					HttpStreamEventArgs e = new HttpStreamEventArgs(helper, stream);
					helper.OnRequestBegin(e);

					// Write the PostData to the stream.
					byte[] buffer = helper.PostData.ToArray();
					if (buffer != null && buffer.Length > 0)
					{
						stream.Write(buffer, 0, buffer.Length);
					}
					stream.Close();
				}
				helper.Request.BeginGetResponse(new AsyncCallback(HttpHelper.BeginResponse), helper);
			}
		}

		private static void BeginResponse(IAsyncResult ar)
		{
			HttpHelper helper = ar.AsyncState as HttpHelper;
			if (helper != null)
			{
				HttpWebResponse response = (HttpWebResponse)helper.Request.EndGetResponse(ar);
				if (response != null)
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						Stream stream = response.GetResponseStream();
						if (stream != null)
						{
							helper.OnResponseComplete(new HttpStreamEventArgs(helper, stream));
							stream.Close();
						}
					}
					else
					{
						helper.OnResponseFailed(new HttpResponseFailedEventArgs(helper, response.StatusDescription));
					}
				}
			}
		}
	}

	public class HttpHelperEventArgs : EventArgs
	{
		public HttpHelper Helper { get; set; }

		public HttpHelperEventArgs(HttpHelper helper)
		{
			this.Helper = helper;
		}
	}

	public delegate void HttpResponseFailedEventHandler(HttpResponseFailedEventArgs e);
	public class HttpResponseFailedEventArgs : HttpHelperEventArgs
	{
		public string Message { get; set; }

		public HttpResponseFailedEventArgs(HttpHelper helper, string message)
			: base(helper)
		{
			this.Message = message;
		}
	}

	public delegate void HttpStreamEventHandler(HttpStreamEventArgs e);
	public class HttpStreamEventArgs : HttpHelperEventArgs
	{
		public Stream Stream { get; set; }

		public HttpStreamEventArgs(HttpHelper helper, Stream stream)
			: base(helper)
		{
			this.Stream = stream;
		}
	}
}
