using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Radiance.Contract
{

	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute(Namespace = "urn:Lionsguard:Radiance:2010:01:schemas", ConfigurationName = "IDepotService")]
	public interface IDepotService
	{

		[System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapNames", ReplyAction = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapNamesResponse")]
		System.IAsyncResult BeginGetMapNames(System.AsyncCallback callback, object asyncState);

		string EndGetMapNames(System.IAsyncResult result);

		[System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapChunk", ReplyAction = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapChunkResponse")]
		System.IAsyncResult BeginGetMapChunk(string mapName, int startX, int startY, bool includeActors, System.AsyncCallback callback, object asyncState);

		Radiance.Contract.MapChunk EndGetMapChunk(System.IAsyncResult result);

		[System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/SubmitError", ReplyAction = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/SubmitErrorResponse")]
		System.IAsyncResult BeginSubmitError(string remoteHost, string errorData, System.AsyncCallback callback, object asyncState);

		void EndSubmitError(System.IAsyncResult result);
	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	public interface IDepotServiceChannel : IDepotService, System.ServiceModel.IClientChannel
	{
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	public partial class GetMapNamesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
	{

		private object[] results;

		public GetMapNamesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return ((string)(this.results[0]));
			}
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	public partial class GetMapChunkCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
	{

		private object[] results;

		public GetMapChunkCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
			base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Radiance.Contract.MapChunk Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return ((Radiance.Contract.MapChunk)(this.results[0]));
			}
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	public partial class DepotServiceClient : System.ServiceModel.ClientBase<IDepotService>, IDepotService
	{

		private BeginOperationDelegate onBeginGetMapNamesDelegate;

		private EndOperationDelegate onEndGetMapNamesDelegate;

		private System.Threading.SendOrPostCallback onGetMapNamesCompletedDelegate;

		private BeginOperationDelegate onBeginGetMapChunkDelegate;

		private EndOperationDelegate onEndGetMapChunkDelegate;

		private System.Threading.SendOrPostCallback onGetMapChunkCompletedDelegate;

		private BeginOperationDelegate onBeginSubmitErrorDelegate;

		private EndOperationDelegate onEndSubmitErrorDelegate;

		private System.Threading.SendOrPostCallback onSubmitErrorCompletedDelegate;

		private BeginOperationDelegate onBeginOpenDelegate;

		private EndOperationDelegate onEndOpenDelegate;

		private System.Threading.SendOrPostCallback onOpenCompletedDelegate;

		private BeginOperationDelegate onBeginCloseDelegate;

		private EndOperationDelegate onEndCloseDelegate;

		private System.Threading.SendOrPostCallback onCloseCompletedDelegate;

		public DepotServiceClient()
		{
		}

		public DepotServiceClient(string endpointConfigurationName) :
			base(endpointConfigurationName)
		{
		}

		public DepotServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		public DepotServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		public DepotServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(binding, remoteAddress)
		{
		}

		public System.Net.CookieContainer CookieContainer
		{
			get
			{
				System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
				if ((httpCookieContainerManager != null))
				{
					return httpCookieContainerManager.CookieContainer;
				}
				else
				{
					return null;
				}
			}
			set
			{
				System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
				if ((httpCookieContainerManager != null))
				{
					httpCookieContainerManager.CookieContainer = value;
				}
				else
				{
					throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
							"ookieContainerBindingElement.");
				}
			}
		}

		public event System.EventHandler<GetMapNamesCompletedEventArgs> GetMapNamesCompleted;

		public event System.EventHandler<GetMapChunkCompletedEventArgs> GetMapChunkCompleted;

		public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SubmitErrorCompleted;

		public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;

		public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		System.IAsyncResult IDepotService.BeginGetMapNames(System.AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginGetMapNames(callback, asyncState);
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		string IDepotService.EndGetMapNames(System.IAsyncResult result)
		{
			return base.Channel.EndGetMapNames(result);
		}

		private System.IAsyncResult OnBeginGetMapNames(object[] inValues, System.AsyncCallback callback, object asyncState)
		{
			return ((IDepotService)(this)).BeginGetMapNames(callback, asyncState);
		}

		private object[] OnEndGetMapNames(System.IAsyncResult result)
		{
			string retVal = ((IDepotService)(this)).EndGetMapNames(result);
			return new object[] {
                retVal};
		}

		private void OnGetMapNamesCompleted(object state)
		{
			if ((this.GetMapNamesCompleted != null))
			{
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.GetMapNamesCompleted(this, new GetMapNamesCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
			}
		}

		public void GetMapNamesAsync()
		{
			this.GetMapNamesAsync(null);
		}

		public void GetMapNamesAsync(object userState)
		{
			if ((this.onBeginGetMapNamesDelegate == null))
			{
				this.onBeginGetMapNamesDelegate = new BeginOperationDelegate(this.OnBeginGetMapNames);
			}
			if ((this.onEndGetMapNamesDelegate == null))
			{
				this.onEndGetMapNamesDelegate = new EndOperationDelegate(this.OnEndGetMapNames);
			}
			if ((this.onGetMapNamesCompletedDelegate == null))
			{
				this.onGetMapNamesCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetMapNamesCompleted);
			}
			base.InvokeAsync(this.onBeginGetMapNamesDelegate, null, this.onEndGetMapNamesDelegate, this.onGetMapNamesCompletedDelegate, userState);
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		System.IAsyncResult IDepotService.BeginGetMapChunk(string mapName, int startX, int startY, bool includeActors, System.AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginGetMapChunk(mapName, startX, startY, includeActors, callback, asyncState);
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		Radiance.Contract.MapChunk IDepotService.EndGetMapChunk(System.IAsyncResult result)
		{
			return base.Channel.EndGetMapChunk(result);
		}

		private System.IAsyncResult OnBeginGetMapChunk(object[] inValues, System.AsyncCallback callback, object asyncState)
		{
			string mapName = ((string)(inValues[0]));
			int startX = ((int)(inValues[1]));
			int startY = ((int)(inValues[2]));
			bool includeActors = ((bool)(inValues[3]));
			return ((IDepotService)(this)).BeginGetMapChunk(mapName, startX, startY, includeActors, callback, asyncState);
		}

		private object[] OnEndGetMapChunk(System.IAsyncResult result)
		{
			Radiance.Contract.MapChunk retVal = ((IDepotService)(this)).EndGetMapChunk(result);
			return new object[] {
                retVal};
		}

		private void OnGetMapChunkCompleted(object state)
		{
			if ((this.GetMapChunkCompleted != null))
			{
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.GetMapChunkCompleted(this, new GetMapChunkCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
			}
		}

		public void GetMapChunkAsync(string mapName, int startX, int startY, bool includeActors)
		{
			this.GetMapChunkAsync(mapName, startX, startY, includeActors, null);
		}

		public void GetMapChunkAsync(string mapName, int startX, int startY, bool includeActors, object userState)
		{
			if ((this.onBeginGetMapChunkDelegate == null))
			{
				this.onBeginGetMapChunkDelegate = new BeginOperationDelegate(this.OnBeginGetMapChunk);
			}
			if ((this.onEndGetMapChunkDelegate == null))
			{
				this.onEndGetMapChunkDelegate = new EndOperationDelegate(this.OnEndGetMapChunk);
			}
			if ((this.onGetMapChunkCompletedDelegate == null))
			{
				this.onGetMapChunkCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetMapChunkCompleted);
			}
			base.InvokeAsync(this.onBeginGetMapChunkDelegate, new object[] {
                    mapName,
                    startX,
                    startY,
                    includeActors}, this.onEndGetMapChunkDelegate, this.onGetMapChunkCompletedDelegate, userState);
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		System.IAsyncResult IDepotService.BeginSubmitError(string remoteHost, string errorData, System.AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginSubmitError(remoteHost, errorData, callback, asyncState);
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		void IDepotService.EndSubmitError(System.IAsyncResult result)
		{
			base.Channel.EndSubmitError(result);
		}

		private System.IAsyncResult OnBeginSubmitError(object[] inValues, System.AsyncCallback callback, object asyncState)
		{
			string remoteHost = ((string)(inValues[0]));
			string errorData = ((string)(inValues[1]));
			return ((IDepotService)(this)).BeginSubmitError(remoteHost, errorData, callback, asyncState);
		}

		private object[] OnEndSubmitError(System.IAsyncResult result)
		{
			((IDepotService)(this)).EndSubmitError(result);
			return null;
		}

		private void OnSubmitErrorCompleted(object state)
		{
			if ((this.SubmitErrorCompleted != null))
			{
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.SubmitErrorCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}

		public void SubmitErrorAsync(string remoteHost, string errorData)
		{
			this.SubmitErrorAsync(remoteHost, errorData, null);
		}

		public void SubmitErrorAsync(string remoteHost, string errorData, object userState)
		{
			if ((this.onBeginSubmitErrorDelegate == null))
			{
				this.onBeginSubmitErrorDelegate = new BeginOperationDelegate(this.OnBeginSubmitError);
			}
			if ((this.onEndSubmitErrorDelegate == null))
			{
				this.onEndSubmitErrorDelegate = new EndOperationDelegate(this.OnEndSubmitError);
			}
			if ((this.onSubmitErrorCompletedDelegate == null))
			{
				this.onSubmitErrorCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSubmitErrorCompleted);
			}
			base.InvokeAsync(this.onBeginSubmitErrorDelegate, new object[] {
                    remoteHost,
                    errorData}, this.onEndSubmitErrorDelegate, this.onSubmitErrorCompletedDelegate, userState);
		}

		private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState)
		{
			return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
		}

		private object[] OnEndOpen(System.IAsyncResult result)
		{
			((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
			return null;
		}

		private void OnOpenCompleted(object state)
		{
			if ((this.OpenCompleted != null))
			{
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}

		public void OpenAsync()
		{
			this.OpenAsync(null);
		}

		public void OpenAsync(object userState)
		{
			if ((this.onBeginOpenDelegate == null))
			{
				this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
			}
			if ((this.onEndOpenDelegate == null))
			{
				this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
			}
			if ((this.onOpenCompletedDelegate == null))
			{
				this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
			}
			base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
		}

		private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState)
		{
			return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
		}

		private object[] OnEndClose(System.IAsyncResult result)
		{
			((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
			return null;
		}

		private void OnCloseCompleted(object state)
		{
			if ((this.CloseCompleted != null))
			{
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}

		public void CloseAsync()
		{
			this.CloseAsync(null);
		}

		public void CloseAsync(object userState)
		{
			if ((this.onBeginCloseDelegate == null))
			{
				this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
			}
			if ((this.onEndCloseDelegate == null))
			{
				this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
			}
			if ((this.onCloseCompletedDelegate == null))
			{
				this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
			}
			base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
		}

		protected override IDepotService CreateChannel()
		{
			return new DepotServiceClientChannel(this);
		}

		private class DepotServiceClientChannel : ChannelBase<IDepotService>, IDepotService
		{

			public DepotServiceClientChannel(System.ServiceModel.ClientBase<IDepotService> client) :
				base(client)
			{
			}

			public System.IAsyncResult BeginGetMapNames(System.AsyncCallback callback, object asyncState)
			{
				object[] _args = new object[0];
				System.IAsyncResult _result = base.BeginInvoke("GetMapNames", _args, callback, asyncState);
				return _result;
			}

			public string EndGetMapNames(System.IAsyncResult result)
			{
				object[] _args = new object[0];
				string _result = ((string)(base.EndInvoke("GetMapNames", _args, result)));
				return _result;
			}

			public System.IAsyncResult BeginGetMapChunk(string mapName, int startX, int startY, bool includeActors, System.AsyncCallback callback, object asyncState)
			{
				object[] _args = new object[4];
				_args[0] = mapName;
				_args[1] = startX;
				_args[2] = startY;
				_args[3] = includeActors;
				System.IAsyncResult _result = base.BeginInvoke("GetMapChunk", _args, callback, asyncState);
				return _result;
			}

			public Radiance.Contract.MapChunk EndGetMapChunk(System.IAsyncResult result)
			{
				object[] _args = new object[0];
				Radiance.Contract.MapChunk _result = ((Radiance.Contract.MapChunk)(base.EndInvoke("GetMapChunk", _args, result)));
				return _result;
			}

			public System.IAsyncResult BeginSubmitError(string remoteHost, string errorData, System.AsyncCallback callback, object asyncState)
			{
				object[] _args = new object[2];
				_args[0] = remoteHost;
				_args[1] = errorData;
				System.IAsyncResult _result = base.BeginInvoke("SubmitError", _args, callback, asyncState);
				return _result;
			}

			public void EndSubmitError(System.IAsyncResult result)
			{
				object[] _args = new object[0];
				base.EndInvoke("SubmitError", _args, result);
			}
		}
	}
}
