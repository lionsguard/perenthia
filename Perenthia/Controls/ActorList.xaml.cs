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

namespace Perenthia.Controls
{
	public partial class ActorList : UserControl, IDropContainer
	{
		public IEnumerable<RdlActor> ItemsSource
		{
			get { return (IEnumerable<RdlActor>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<RdlActor>), typeof(ActorList), new PropertyMetadata(null, new PropertyChangedCallback(ActorList.OnItemsSourcePropertyChanged)));
		private static void OnItemsSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ActorList).BindActors();
		}

		public bool EnableDrag
		{
			get { return (bool)GetValue(EnableDragProperty); }
			set { SetValue(EnableDragProperty, value); }
		}
		public static readonly DependencyProperty EnableDragProperty = DependencyProperty.Register("EnableDrag", typeof(bool), typeof(ActorList), new PropertyMetadata(false, new PropertyChangedCallback(ActorList.OnEnableDragPropertyChanged)));
		private static void OnEnableDragPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ActorList).BindActors();
		}

		public DragDropManager DragDropManager
		{
			get { return (DragDropManager)GetValue(DragDropManagerProperty); }
			set { SetValue(DragDropManagerProperty, value); }
		}
		public static readonly DependencyProperty DragDropManagerProperty = DependencyProperty.Register("DragDropManager", typeof(DragDropManager), typeof(ActorList), new PropertyMetadata(null));	
			

		public event ActorEventHandler ActorDrop = delegate { };

        public event ActorEventHandler ActorClick = delegate { };
			
		public ActorList()
		{
			InitializeComponent();
		}

		private void BindActors()
		{
			if (this.ItemsSource != null)
			{
				lstActors.Children.Clear();
				foreach (var actor in this.ItemsSource)
				{
					ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), actor.Properties.GetValue<string>("ObjectType"), true);
					if (type == ObjectType.Mobile || type == ObjectType.Player)
					{
						AvatarListItem avatarItem = AvatarListItem.Create(actor);
						//avatarItem.Action += new ActionEventHandler(OnActorItemAction);
						if (this.EnableDrag)
						{
							avatarItem.BeginDrag += new BeginDragEventHandler(OnDroppableBeginDrag);
                            avatarItem.ActorDrop += new ActorEventHandler(OnAvatarListItemActorDrop);
                            avatarItem.Click += new ActorEventHandler(OnAvatarListItemClick);
						}
						lstActors.Children.Add(avatarItem);
					}
					else if (type == ObjectType.Actor)
					{
						ItemListItem listItem = ItemListItem.Create(actor);
						//listItem.Action += new ActionEventHandler(OnActorItemAction);
						if (this.EnableDrag)
						{
							listItem.BeginDrag += new BeginDragEventHandler(OnDroppableBeginDrag);
						}
						lstActors.Children.Add(listItem);
					}
				}
			}
		}

        private void OnAvatarListItemClick(object sender, ActorEventArgs e)
        {
            this.ActorClick(this, e);
        }

        private void OnAvatarListItemActorDrop(object sender, ActorEventArgs e)
        {
            this.ActorDrop(this, e);
        }

		private void OnDroppableBeginDrag(object sender, BeginDragEventArgs e)
		{
			if (this.DragDropManager != null)
			{
				this.DragDropManager.BeginDrag(e.Droppable, e.MousePosition);
			}
		}

		#region IDropContainer Members

		public void Drop(IDroppable item)
		{
			if (item is IActorListItem)
			{
				this.ActorDrop(this, new ActorEventArgs { Actor = (item as IActorListItem).Actor });
			}
		}

		#endregion
	}
}
