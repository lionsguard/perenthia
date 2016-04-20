using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Screens
{
    public partial class GameOfflineScreen : UserControl, IScreen
    {
		private Timer _timer = null;

        public GameOfflineScreen()
        {
			this.Loaded += new RoutedEventHandler(GameOfflineScreen_Loaded);
            InitializeComponent();
        }

		void GameOfflineScreen_Loaded(object sender, RoutedEventArgs e)
		{
			ServerManager.Instance.Reset();
			ServerManager.Instance.Response += new ServerResponseEventHandler(Instance_Response);

			_timer = new Timer(new TimerCallback(this.TimerCallBack), null, 0, 300000);
		}

		private void TimerCallBack(object state)
		{
			ServerManager.Instance.SendUserCommand("ISONLINE");
		}

		void Instance_Response(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessTags(e.Tags));
		}

		private void ProcessTags(RdlTagCollection tags)
		{
			var tag = tags.GetTags<RdlTag>("ISONLINE", "ISONLINE");
			if (tag.Count > 0)
			{
				if (_timer != null) _timer.Dispose();
				ScreenManager.SetScreen(new HomeScreen());
			}
		}

		#region IScreen Members

		public UIElement Element
		{
			get { return this; }
		}

		public void OnAddedToHost()
		{
		}

		public void OnRemovedFromHost()
		{
		}

		#endregion
	}
}
