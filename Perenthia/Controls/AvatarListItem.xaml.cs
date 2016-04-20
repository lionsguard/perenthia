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

using Lionsguard;
using Radiance;
using Radiance.Markup;
using Perenthia.Dialogs;
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class AvatarListItem : UserControl, IActorListItem, IDropContainer
	{
		public RdlActor Actor
		{
			get { return (RdlActor)GetValue(ActorProperty); }
			set { SetValue(ActorProperty, value); }
		}
		public static readonly DependencyProperty ActorProperty = DependencyProperty.Register("Actor", typeof(RdlActor), typeof(AvatarListItem), new PropertyMetadata(null, new PropertyChangedCallback(AvatarListItem.OnActorPropertyChanged)));
		private static void OnActorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as AvatarListItem).SetDetails();
		}

		public event ActionEventHandler Action = delegate { };

        public event ActorEventHandler ActorDrop = delegate { };

        public event ActorEventHandler Click = delegate { };

		private AvatarDetails _details = null;
		
		public AvatarListItem()
		{
			this.Loaded += new RoutedEventHandler(AvatarListItem_Loaded);
			this.MouseLeftButtonDown += new MouseButtonEventHandler(AvatarListItem_MouseLeftButtonDown);
			InitializeComponent();
		}

		void AvatarListItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.Actor != null)
			{
                this.Click(this, new ActorEventArgs { Actor = this.Actor });

				Logger.LogDebug("AvatarListItem Mouse Down");
				this.BeginDrag(this, new BeginDragEventArgs { Droppable = this, MousePosition = e.GetPosition(null) });
			}
		}

		void AvatarListItem_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void SetDetails()
		{
			if (this.Actor != null)
			{
				MainImage.Source = this.GetImageSource();

				bool isProperName = this.Actor.Properties.GetValue<bool>("HasProperName");

				// TODO: Display some type of icon when a dead avatar is displayed.
				// TODO: Also, if the avatar is dead then display the LOOT icon, if the target id of the dead
				// avatar equals the current player.
				if (this.Actor.Properties.GetValue<bool>("IsDead"))
				{
					NameLabel.Text = String.Concat(this.Actor.Name.AUpper(isProperName), " (DEAD)");

					// Hide the "Set Target" button and display the LOOT button.
					btnAttack.Visibility = Visibility.Collapsed;
					btnTell.Visibility = Visibility.Collapsed;// Can't talk to something that is dead.

					btnLoot.Visibility = Visibility.Visible;
				}
				else
				{
					NameLabel.Text = this.Actor.Name.A(isProperName);
				}

				if (!this.Actor.Properties.GetValue<bool>("CanAttack"))
				{
					btnAttack.Visibility = Visibility.Collapsed;
				}

				if (MobileHelper.IsGoodsAndServicesSeller(this.Actor))
				{
					btnCommerce.Visibility = Visibility.Visible;
				}
				else
				{
					btnCommerce.Visibility = Visibility.Collapsed;
				}

				if (MobileHelper.IsQuestGiver(this.Actor))
				{
					btnQuests.Visibility = Visibility.Visible;
				}
				else
				{
					btnQuests.Visibility = Visibility.Collapsed;
				}
			}
		}

		private ImageSource GetImageSource()
		{
			string imageUri = this.Actor.Properties.GetValue<string>("ImageUri");
			if (String.IsNullOrEmpty(imageUri))
			{
				imageUri = String.Format(Asset.AVATAR_FORMAT,
					this.Actor.Properties.GetValue<string>("Race"),
					this.Actor.Properties.GetValue<string>("Gender"));
			}
			return Asset.GetImageSource(imageUri);
		}

		private void btnCommerce_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Action(this, new ActionEventArgs(Actions.Goods, this.Actor.ID, this.Actor.Name));
		}

		private void btnAttack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Action(this, new ActionEventArgs(Actions.Target, this.Actor.ID, this.Actor.Name));
		}

		private void btnTell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Action(this, new ActionEventArgs(Actions.Tell, this.Actor.ID, this.Actor.Name));
		}

		private void btnLoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Action(this, new ActionEventArgs(Actions.Loot, this.Actor.ID, this.Actor.Name));
		}

		private void btnQuests_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Action(this, new ActionEventArgs(Actions.Quests, this.Actor.ID, this.Actor.Name));
		}

		public static AvatarListItem Create(RdlActor avatar)
		{
			AvatarListItem item = new AvatarListItem();
			item.Actor = avatar;
			return item;
		}

		#region IDroppable Members

		public event BeginDragEventHandler BeginDrag = delegate { };

		public UIElement GetDragCursor()
		{
			Image img = new Image();
			img.Source = this.GetImageSource();
			img.Width = 16;
			img.Height = 16;
			Border border = new Border();
			border.Background = Brushes.DialogFillBrush;
			border.BorderBrush = Brushes.BorderBrush;
			border.Padding = new Thickness(2);
			border.Width = 18;
			border.Height = 18;
			border.Child = img;
			return border;
		}

		#endregion

		private void NameLabel_MouseEnter(object sender, MouseEventArgs e)
		{
			_details = new AvatarDetails();
			_details.Show(new Avatar(this.Actor));
			PopupManager.Add(_details, e.GetPosition(null));
		}

		private void NameLabel_MouseLeave(object sender, MouseEventArgs e)
		{
			if (_details != null)
			{
				_details.Hide();
			}
			PopupManager.Remove();
		}

		#region IDropContainer Members

		public void Drop(IDroppable item)
		{
			if (item is ItemListItem)
			{
				RdlActor actor = (item as ItemListItem).Actor;
                if (actor.Properties.GetValue<ObjectType>("ObjectType") == ObjectType.Quest)
				{
					QuestFacilitatorDialog diag = new QuestFacilitatorDialog();
					diag.Tag = actor;
					diag.QuestFacilitatorTypeSelected += new QuestFacilitatorTypeSelectedEventHandler(diag_QuestFacilitatorTypeSelected);
					PopupManager.Add(diag, new Point(354, 57)); // Admin screen position.
				}
				else
				{
					actor.OwnerID = this.Actor.ID;
                    this.ActorDrop(this, new ActorEventArgs { Actor = actor });
                    //// Args:
                    //// 0 = Item
                    //// 1 = Target
                    //ServerManager.Instance.SendCommand("ADMINDROPITEM", actor.Name, this.Actor.ID);
				}
			}
		}

		void diag_QuestFacilitatorTypeSelected(object sender, QuestFacilitatorTypeSelectedEventArgs e)
		{
			RdlActor actor = (sender as QuestFacilitatorDialog).Tag as RdlActor;
            PopupManager.Remove();
            this.ActorDrop(this, new ActorEventArgs { Actor = actor });
            //// Args:
            //// 0 = Item
            //// 1 = Target
            //// 2 = Arg0
            //ServerManager.Instance.SendCommand("ADMINDROPITEM", actor.Name, this.Actor.ID, e.Type);
		}

		#endregion
	}
}
