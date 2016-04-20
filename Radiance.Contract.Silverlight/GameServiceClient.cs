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
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Radiance.Contract
{
	public partial class GameServiceClient : DuplexClientBase<IGameService>, IGameService
	{

		private BeginOperationDelegate onBeginProcessDelegate;

		private EndOperationDelegate onEndProcessDelegate;

		private System.Threading.SendOrPostCallback onProcessCompletedDelegate;

		private bool useGeneratedCallback;

		private BeginOperationDelegate onBeginOpenDelegate;

		private EndOperationDelegate onEndOpenDelegate;

		private System.Threading.SendOrPostCallback onOpenCompletedDelegate;

		private BeginOperationDelegate onBeginCloseDelegate;

		private EndOperationDelegate onEndCloseDelegate;

		private System.Threading.SendOrPostCallback onCloseCompletedDelegate;

		public GameServiceClient(System.ServiceModel.EndpointAddress remoteAddress)
			: this(new GameServiceClientCallback(),
				new CustomBinding(
					new PollingDuplexBindingElement(),
					new BinaryMessageEncodingBindingElement(),
					new HttpTransportBindingElement()),
				remoteAddress)
		{
		}

		public GameServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(callbackInstance, binding, remoteAddress)
		{
		}

		public GameServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			this(new GameServiceClientCallback(), binding, remoteAddress)
		{
		}

		private GameServiceClient(GameServiceClientCallback callbackImpl, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			this(new System.ServiceModel.InstanceContext(callbackImpl), binding, remoteAddress)
		{
			useGeneratedCallback = true;
			callbackImpl.Initialize(this);
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

		public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> ProcessCompleted;

		public event System.EventHandler<PacketReceivedEventArgs> ReceiveReceived;

		public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;

		public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		System.IAsyncResult IGameService.BeginProcess(byte[] data, System.AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginProcess(data, callback, asyncState);
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		void IGameService.EndProcess(System.IAsyncResult result)
		{
			base.Channel.EndProcess(result);
		}

		private System.IAsyncResult OnBeginProcess(object[] inValues, System.AsyncCallback callback, object asyncState)
		{
			byte[] data = ((byte[])(inValues[0]));
			return ((IGameService)(this)).BeginProcess(data, callback, asyncState);
		}

		private object[] OnEndProcess(System.IAsyncResult result)
		{
			((IGameService)(this)).EndProcess(result);
			return null;
		}

		private void OnProcessCompleted(object state)
		{
			if ((this.ProcessCompleted != null))
			{
				InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
				this.ProcessCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
			}
		}

		public void ProcessAsync(byte[] data)
		{
			this.ProcessAsync(data, null);
		}

		public void ProcessAsync(byte[] data, object userState)
		{
			if ((this.onBeginProcessDelegate == null))
			{
				this.onBeginProcessDelegate = new BeginOperationDelegate(this.OnBeginProcess);
			}
			if ((this.onEndProcessDelegate == null))
			{
				this.onEndProcessDelegate = new EndOperationDelegate(this.OnEndProcess);
			}
			if ((this.onProcessCompletedDelegate == null))
			{
				this.onProcessCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnProcessCompleted);
			}
			base.InvokeAsync(this.onBeginProcessDelegate, new object[] {
                    data}, this.onEndProcessDelegate, this.onProcessCompletedDelegate, userState);
		}

		private void OnReceiveReceived(object state)
		{
			if ((this.ReceiveReceived != null))
			{
				object[] results = ((object[])(state));
				this.ReceiveReceived(this, new PacketReceivedEventArgs(results, null, false, null));
			}
		}

		private void VerifyCallbackEvents()
		{
			if (((this.useGeneratedCallback != true)
						&& (this.ReceiveReceived != null)))
			{
				throw new System.InvalidOperationException("Callback events cannot be used when the callback InstanceContext is specified. Pl" +
						"ease choose between specifying the callback InstanceContext or subscribing to th" +
						"e callback events.");
			}
		}

		private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState)
		{
			this.VerifyCallbackEvents();
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

		protected override IGameService CreateChannel()
		{
			return new GameServiceClientChannel(this);
		}

		private class GameServiceClientCallback : object, IGameServiceCallback
		{

			private GameServiceClient proxy;

			public void Initialize(GameServiceClient proxy)
			{
				this.proxy = proxy;
			}

			public void Receive(Message msg)
			{
				this.proxy.OnReceiveReceived(new object[] { msg	});
			}
		}

		private class GameServiceClientChannel : ChannelBase<IGameService>, IGameService
		{

			public GameServiceClientChannel(System.ServiceModel.DuplexClientBase<IGameService> client) :
				base(client)
			{
			}

			public System.IAsyncResult BeginProcess(byte[] data, System.AsyncCallback callback, object asyncState)
			{
				object[] _args = new object[1];
				_args[0] = data;
				System.IAsyncResult _result = base.BeginInvoke("Process", _args, callback, asyncState);
				return _result;
			}

			public void EndProcess(System.IAsyncResult result)
			{
				object[] _args = new object[0];
				base.EndInvoke("Process", _args, result);
			}
		}
	}
}
