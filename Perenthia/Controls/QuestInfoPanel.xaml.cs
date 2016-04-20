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

using Radiance;
using Radiance.Markup;

namespace Perenthia.Controls
{
	public partial class QuestInfoPanel : UserControl
	{
		public RdlActor TargetQuest { get; set; }
		public RdlActor PlayerQuest { get; set; }	
			
		public event RoutedEventHandler Click = delegate { };
				
		public QuestInfoPanel()
		{
			InitializeComponent();
			this.MouseLeftButtonDown += new MouseButtonEventHandler(QuestInfoPanel_MouseLeftButtonDown);
		}

		private void QuestInfoPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Click(this, e);
		}

		public void Refresh()
		{
			this.BindDetails();
		}

		private void BindDetails()
		{
			if (this.TargetQuest != null)
			{
				lblName.Text = this.TargetQuest.Name;

				bool displayDetails = true;
				if (this.PlayerQuest != null)
				{
					// Display the quest details based on some values on the player quest instance.
					if (this.PlayerQuest.Properties.GetValue<bool>("IsComplete")
						|| this.PlayerQuest.Properties.GetValue<bool>("IsFinished"))
					{
						displayDetails = false;
					}
				}
				else
				{
					// Player quest is null so the target quest may be the player quest instance.
					if (this.TargetQuest.Properties.GetValue<bool>("IsComplete")
						|| this.TargetQuest.Properties.GetValue<bool>("IsFinished"))
					{
						displayDetails = false;
					}
				}

				if (displayDetails)
				{
					// not finished and not complete and starts with target.
					lblDetails.Text = String.Format("Min Level = {0}, Max Level = {1}, Experience = {2}",
						this.TargetQuest.Properties.GetValue<int>("MinimumLevel"),
						this.TargetQuest.Properties.GetValue<int>("MaximumLevel"),
						this.TargetQuest.Properties.GetValue<int>("RewardExperience"));
				}
				else
				{
					lblDetails.Text = "COMPLETE";
				}

				imgMain.Source = Asset.GetImageSource(this.TargetQuest.Properties.GetValue<string>("ImageUri"));
			}
		}
	}
}
