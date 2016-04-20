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
using System.IO.IsolatedStorage;

namespace Perenthia.Windows
{
	public partial class IncreaseIsoQuotaWindow : ChildWindow
	{
		public IncreaseIsoQuotaWindow()
		{
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					if (store.Quota < StorageManager.RequiredQuota)
					{
						// Request more quota space.
						if (!store.IncreaseQuotaTo(StorageManager.RequiredQuota))
						{
							// The user clicked NO to the
							// host's prompt to approve the quota increase.
						}
						else
						{
							// The user clicked YES to the
							// host's prompt to approve the quota increase.
							StorageManager.AcceptIncrease();
						}
					}
				}
			}
			catch (IsolatedStorageException)
			{
				// TODO: Handle that store could not be accessed.
			}
			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}

