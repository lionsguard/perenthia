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
	public partial class ContextMenu : UserControl
	{
		public enum ContextMenuType
		{
			Player,
			Mobile,
			Item,
			Action,
			Equipment,
		}


		public RdlActor Actor { get; set; }
		public ContextMenuType Type { get; set; }	

		public event ActionEventHandler Click = delegate { };
        public event CreateActionEventHandler CreateAction = delegate { return null; };

		public ContextMenu()
			: this(ContextMenuType.Item, null)
		{
		}

		public ContextMenu(ContextMenuType type, RdlActor actor)
		{
			this.Loaded += new RoutedEventHandler(ContextMenu_Loaded);
			InitializeComponent();
			this.Type = type;
			this.Actor = actor;
		}

		private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
		{
			switch (this.Type)
			{
				case ContextMenuType.Player:
					this.CreateLink(String.Format("Target {0}", this.Actor.Name), Actions.Target);
					this.CreateLink(String.Format("Send Tell to {0}", this.Actor.Name), Actions.Tell);
					break;
				case ContextMenuType.Mobile:
					this.CreateLink(String.Format("Target {0}", this.Actor.Name.The(this.Actor.Properties.GetValue<bool>("HasProperName"))), Actions.Target);
					break;
				case ContextMenuType.Item:
				case ContextMenuType.Equipment:
					if (this.Actor.IsUsable())
					{
						this.CreateLink(String.Format("Use {0}", this.Actor.Name.The(false)), Actions.Use);
					}
					if (this.Actor.EquipLocation() != EquipLocation.None)
					{
						if (this.Actor.IsEquipped())
						{
							this.CreateLink(String.Format("Unequip {0}", this.Actor.Name.The(false)), Actions.Unequip);
						}
						else
						{
							this.CreateLink(String.Format("Equip {0}", this.Actor.Name.The(false)), Actions.Equip);
						}
					}
					this.CreateLink(String.Format("Drop {0}", this.Actor.Name.The(false)), Actions.Drop);
					break;
				case ContextMenuType.Action:
                    if (this.Actor.IsUsable())
                    {
                        this.CreateLink(String.Format("Use {0}", this.Actor.Name.The(false)), Actions.Use);
                    }
					this.CreateLink("Clear Action", Actions.SetAction);
					break;
			}
		}

		private void CreateLink(string text, string action)
		{
			HyperlinkButton link = new HyperlinkButton();
			link.Foreground = Brushes.LinkBrush;
			link.Content = text;
			link.Tag = action;
			link.Click += new RoutedEventHandler(link_Click);
			ctlLinks.Children.Add(link);
		}

		private void link_Click(object sender, RoutedEventArgs e)
		{
			HyperlinkButton link = sender as HyperlinkButton;
			if (link != null)
			{
                string actionName = link.Tag.ToString();
                ActionEventArgs args = this.CreateAction(new CreateActionEventArgs { ActionName = actionName, Actor = this.Actor });
                if (args == null)
                {
                    args = new ActionEventArgs(actionName, this.Actor.ID, this.Actor.Name);
                }
				this.Click(this, args);
			}
		}

		private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
		{
		}
	}

    public delegate ActionEventArgs CreateActionEventHandler(CreateActionEventArgs e);
    public class CreateActionEventArgs : EventArgs
    {
        public string ActionName { get; set; }
        public RdlActor Actor { get; set; } 
    }
}
