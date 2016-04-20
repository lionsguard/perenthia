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
	public partial class RoomPanel : UserControl
	{
		private RdlActorDictionary _actors = new RdlActorDictionary();

		public Point3 Location
		{
			get { return (Point3)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(Point3), typeof(RoomPanel), new PropertyMetadata(Point3.Empty, new PropertyChangedCallback(RoomPanel.OnLocationPropertyChanged)));
		private static void OnLocationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)	
		{
			(obj as RoomPanel).Refresh();
		}
			
		#region Events
		public event ActionEventHandler Action = delegate { };
		#endregion

		public RoomPanel()
		{
			InitializeComponent();
		}

		public void Clear()
		{
			_actors.Clear();
			ctlItems.Children.Clear();
		}

		public void Remove(int id)
		{
			_actors.Remove(id);
			this.Refresh();
		}

		public void SetItems(List<RdlActor> actors)
		{
			foreach (var actor in actors)
			{
				if (_actors.ContainsKey(actor.ID))
				{
					_actors[actor.ID] = actor;
				}
				else
				{
					_actors.Add(actor.ID, actor);
				}
			}
			this.Refresh();
		}

		private void Refresh()
		{
			// Remove any actors not in the current location.
			var removes = _actors.Where(a => new Point3(a.Value.Properties.GetValue<int>("X"),
					a.Value.Properties.GetValue<int>("Y"),
					a.Value.Properties.GetValue<int>("Z")) != this.Location).Select(a => a.Value.ID).ToList();
			foreach (var id in removes)
			{
				_actors.Remove(id);
			}

			ctlItems.Children.Clear();
			foreach (var actor in _actors.Values)
			{
				ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), actor.Properties.GetValue<string>("ObjectType"), true);
				if (type == ObjectType.Mobile || type == ObjectType.Player)
				{
					AvatarListItem avatarItem = AvatarListItem.Create(actor);
					avatarItem.Action += new ActionEventHandler(OnActorItemAction);
					ctlItems.Children.Add(avatarItem);
				}
				else if (type == ObjectType.Actor)
				{
					ItemListItem listItem = ItemListItem.Create(actor);
					listItem.Action += new ActionEventHandler(OnActorItemAction);
					ctlItems.Children.Add(listItem);
				}
			}
		}

		void OnActorItemAction(object sender, ActionEventArgs e)
		{
			this.Action(sender, e);
		}
	}
}
