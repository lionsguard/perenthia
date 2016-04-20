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
using SilverlightLoader;

namespace Perenthia.Loader
{
	/// <summary>
	/// This class is the main loader UI and implements the ISilverlightLoader interface
	/// </summary>
	public partial class Loader : UserControl, ISilverlightLoader
	{
		public Loader()
		{
			InitializeComponent();
		}

		#region ISilverlightLoader Members

		// called when download of all packages starts(called once)
		public void initCallback(List<Uri> packageSourceList)
		{
			// * note we only take the size here because if they are streaming sources the url will change later. 
			// so we get the real url's later in downloadStartCallback.
			m_packageSourceList = new Dictionary<Uri, ProgressCtrl>(packageSourceList.Count);
			m_packageDownloadCount = packageSourceList.Count;
		}

		// called when download of each package/file starts
		public void downloadStartCallback(Uri packageSource)
		{
			//dynamicaly generate a progress control for each file we download and add it to UI
			ProgressCtrl progressCtrl = new ProgressCtrl();
			string packageName = packageSource.ToString();
			if (packageName.Length > 50)
				packageName = ".." + packageName.Substring(packageName.Length - 50);
			progressCtrl.Name = packageName;
			progressCtrl.Blink.AutoReverse = true;
			progressCtrl.Blink.Begin();
			DownloadListStackCtrl.Children.Add(progressCtrl);
			m_packageSourceList.Add(packageSource, progressCtrl);
		}

		// called on download progress of each package/file
		public void downloadProgressCallback(Uri packageSource, DownloadProgressEventArgs eventArgs)
		{
			float percentageDownloadedAcurate = ((float)eventArgs.BytesReceived / eventArgs.TotalBytesToReceive) * 100;
			ProgressCtrl progressCtrl = m_packageSourceList[packageSource];
			string packageName = progressCtrl.Name;
			progressCtrl.LoadingTextCtrl.Text = "Downloaded " + percentageDownloadedAcurate.ToString("##0") + "%";
			progressCtrl.ProgressBarCtrl.Value = eventArgs.ProgressPercentage;
		}

		// called on download complete of each package/file
		public void downloadCompleteCallback(Uri packageSource, DownloadCompleteEventArgs e)
		{
			m_packageDownloadCount--;
			// if download is complete set source to a package of our choice
			if (m_packageDownloadCount <= 0)
			{
				// ! note that for the demo's sake we are just setting the active xap to be the first xap on the list.
				//   you should probably modify this if you have more then one xap on the list !
				foreach (Uri source in m_packageSourceList.Keys)
					if (source.ToString().EndsWith(".xap"))
					{
						// this will unload the the loader from the page and cause the package source to become the active xap file on page
						// ! this should be the last loader operation after that it will start the unload process !
						XapUtil.setCurrentXapFile(source);
						break;
					}
			}
		}

		#endregion

		// data
		Dictionary<Uri, ProgressCtrl> m_packageSourceList;
		int m_packageDownloadCount;
	}
}
