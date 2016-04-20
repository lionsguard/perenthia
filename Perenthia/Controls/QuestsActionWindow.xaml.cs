using System;
using System.Collections;
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
using System.Text;

using Lionsguard;
using Radiance;
using Radiance.Markup;
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class QuestsActionWindow : UserControl, IActionWindow
	{
		public RdlActor Quest
		{
			get { return (RdlActor)GetValue(QuestProperty); }
			set { SetValue(QuestProperty, value); }
		}
		public static readonly DependencyProperty QuestProperty = DependencyProperty.Register("Quest", typeof(RdlActor), typeof(QuestsActionWindow), new PropertyMetadata(null, new PropertyChangedCallback(QuestsActionWindow.OnQuestPropertyChanged)));
		private static void OnQuestPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as QuestsActionWindow).LoadQuest();
		}

		public Avatar Target
		{
			get { return (Avatar)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Avatar), typeof(QuestsActionWindow), new PropertyMetadata(null, new PropertyChangedCallback(QuestsActionWindow.OnTargetPropertyChanged)));
		private static void OnTargetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as QuestsActionWindow).BindQuests();
		}

		public bool IsLoading
		{
			get { return ctlLoading.Visibility == Visibility.Visible; }
		}

		public Avatar Player { get; set; }	
			
		public QuestsActionWindow()
		{
			InitializeComponent();
		}

		#region IActionWindow Members
		public Window ParentWindow { get; set; }

		public event ActionEventHandler Action = delegate { };

		public void Load(ActionEventArgs args)
		{
		}
		#endregion

		private void btnAccept_Click(object sender, RoutedEventArgs e)
		{
			ActionEventArgs args = new ActionEventArgs(Actions.StartQuest, this.Quest.ID, this.Quest.Name);
			args.Args.Add(this.Target.ID);
			this.Action(this, args);
			this.BindQuests();
			this.ParentWindow.Close();
		}

		private void btnComplete_Click(object sender, RoutedEventArgs e)
		{
			ActionEventArgs args = new ActionEventArgs(Actions.CompleteQuest, this.Quest.Name, this.Quest.Name);
			//args.Args.Add(this.Target.ID);
			this.Action(this, args);
			this.Quest = null;
			this.BindQuests();
			this.ParentWindow.Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Quest = null;
			this.BindQuests();
		}

		private void OnQuestInfoPanelClick(object sender, RoutedEventArgs e)
		{
			QuestInfoPanel info = (sender as QuestInfoPanel);
			// Load the details.
			if (this.Target != null)
			{
				this.Quest = info.TargetQuest;
			}
		}

		public void ShowLoader()
		{
			ctlLoading.Visibility = Visibility.Visible;
		}

		public void HideLoader()
		{
			ctlLoading.Visibility = Visibility.Collapsed;
		}

		public void Refresh()
		{
			this.Quest = null;
			this.BindQuests();
			this.HideLoader();
		}

		private void BindQuests()
		{
			if (this.Target != null && this.Player != null)
			{
				QuestDetailContainer.Visibility = Visibility.Collapsed;
				QuestListContainer.Visibility = Visibility.Visible;
				lstQuests.Children.Clear();
				foreach (var item in this.Target.Inventory.GetQuests())
				{
					RdlActor playerQuest = this.Player.Inventory.GetQuests().Where(q => q.Name == item.Name).FirstOrDefault();

					if (playerQuest != null)
					{
						// Do not display quests for which the player has already completed.
						if (playerQuest.Properties.GetValue<bool>("IsComplete"))
							continue;

						// Do not display quests where the target is the endswith value and the player has not finished 
						// nor has the player started quest.
						if ((!playerQuest.Properties.GetValue<bool>("IsFinished") || !playerQuest.Properties.GetValue<bool>("IsStarted"))
							&& playerQuest.Properties.GetValue<int>(String.Format("EndsWith_{0}",
								this.Target.ID)) == this.Target.ID)
							continue;
					}

					if (!String.IsNullOrEmpty(item.Properties.GetValue<string>("ParentQuestName")))
					{
						// If the player has not completed the parent quest, do not display this
						// quest as an option.
						// TODO: This is inefficient, need a better way to query this.
						if (this.Player.Inventory.GetQuests().Where(q =>
							q.Name == item.Properties.GetValue<string>("ParentQuestName")
							&& !q.Properties.GetValue<bool>("IsComplete")).Count() > 0)
						{
							continue;
						}
					}

					QuestInfoPanel pnl = new QuestInfoPanel();
					pnl.PlayerQuest = playerQuest;
					pnl.TargetQuest = item;
					pnl.Click += new RoutedEventHandler(OnQuestInfoPanelClick);
					pnl.Refresh();
					lstQuests.Children.Add(pnl);
				}
			}
			else
			{
				this.ShowLoader();
			}
		}

		private void LoadQuest()
		{
			// Quest will be the target's quest instance, not the player's.
			if (this.Quest != null)
			{
				QuestDetailContainer.Visibility = Visibility.Visible;
				QuestListContainer.Visibility = Visibility.Collapsed;

				lblQuestName.Text = this.Quest.Name;

				lblMessage.Text = this.Quest.Description;

				// Reward items
				lblRewardItems.Text = ItemHelper.GetQuestRewardItemsDisplay(this.Quest);

				// Currency
				ctlCurrency.Currency = new Currency(this.Quest.Properties.GetValue<int>("RewardCurrency"));

				// Emblem
				ctlCurrency.Emblem = this.Quest.Properties.GetValue<int>("RewardEmblem");

				// Load the player's quest instance.
				bool showComplete = false;
				if (this.Player != null && this.Target != null)
				{
					RdlActor playerQuest = this.Player.Inventory.GetQuests().Where(q => q.Name == this.Quest.Name).FirstOrDefault();
					if (playerQuest != null)
					{
						if ((playerQuest.Properties.GetValue<bool>("IsFinished") && !playerQuest.Properties.GetValue<bool>("IsComplete"))
							&& this.Quest.Properties.GetValue<int>(String.Format("EndsWith_{0}",
								this.Target.ID)) == this.Target.ID)
						{
							// Quest is finished, not complete and ends with the current target, display complete
							// button so player can complete the quest and gain the rewards.
							showComplete = true;
							lblMessage.Text = this.Quest.Properties.GetValue<string>("CompletedMessage");
						}
					}
				}

				if (showComplete)
				{
					btnComplete.Visibility = Visibility.Visible;
				}
				else
				{
					btnAccept.Visibility = Visibility.Visible;
				}
			}
			else
			{
				this.BindQuests();
			}
		}
	}
}
