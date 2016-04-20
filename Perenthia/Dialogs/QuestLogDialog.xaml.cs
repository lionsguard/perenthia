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

using Radiance;
using Radiance.Markup;

using Perenthia.Controls;
using Perenthia.Models;

namespace Perenthia.Dialogs
{
	public partial class QuestLogDialog : UserControl
	{
		public Avatar Player
		{
			get { return (Avatar)GetValue(PlayerProperty); }
			set { SetValue(PlayerProperty, value); }
		}
		public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register("Player", typeof(Avatar), typeof(QuestLogDialog), new PropertyMetadata(null, new PropertyChangedCallback(QuestLogDialog.OnPlayerPropertyChanged)));
		private static void OnPlayerPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)	
		{
			(obj as QuestLogDialog).Refresh();
		}
			
		public QuestLogDialog()
		{
			InitializeComponent();
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
			if (this.Player != null)
			{
				var quests = this.Player.Inventory.GetQuests();

				// Active Quests
				lstActive.Children.Clear();
				RdlActor lastQuest = null;
				foreach (var quest in quests.Where(q => !q.Properties.GetValue<bool>("IsComplete")))
				{
					QuestInfoPanel pnl = new QuestInfoPanel();
					pnl.TargetQuest = quest;
					pnl.Click += new RoutedEventHandler(OnQuestInfoPanelClick);
					pnl.Refresh();
					lstActive.Children.Add(pnl);

					lastQuest = quest;
				}
				if (lastQuest != null)
				{
					this.LoadQuest(lastQuest);
				}

				// Compelted Quests
				lstCompleted.Children.Clear();
				foreach (var quest in quests.Where(q => q.Properties.GetValue<bool>("IsComplete")))
				{
					QuestInfoPanel pnl = new QuestInfoPanel();
					pnl.TargetQuest = quest;
					pnl.Refresh();
					lstCompleted.Children.Add(pnl);
				}

				// Hide the loading window.
				this.HideLoader();
			}
		}

		private void OnQuestInfoPanelClick(object sender, RoutedEventArgs e)
		{
			if (this.Player != null)
			{
				QuestInfoPanel info = (sender as QuestInfoPanel);
				var quest = info.TargetQuest;
				if (quest != null)
				{
					this.LoadQuest(quest);
				}
			}
		}

		private void LoadQuest(RdlActor quest)
		{
			// Description
			lblDescription.Text = quest.Description;

			// Rewards
			lblRewards.Text = ItemHelper.GetQuestRewardItemsDisplay(quest);

			// Currency
			ctlCurrency.Currency = new Currency(quest.Properties.GetValue<int>("RewardCurrency"));

			// Emblem
			ctlCurrency.Emblem = quest.Properties.GetValue<int>("RewardEmblem");
		}
	}
}
