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

using Perenthia.Controls;

namespace Perenthia.Dialogs
{
	public partial class BagDialog : UserControl, IDropContainer
	{
		public RdlActor Item
		{
			get { return (RdlActor)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(RdlActor), typeof(BagDialog), new PropertyMetadata(null, new PropertyChangedCallback(BagDialog.OnItemPropertyChanged)));
		private static void OnItemPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as BagDialog).SetCapacity();
		}

		public IEnumerable<RdlActor> Contents
		{
			get { return (IEnumerable<RdlActor>)GetValue(ContentsProperty); }
			set { SetValue(ContentsProperty, value); }
		}
		public static readonly DependencyProperty ContentsProperty = DependencyProperty.Register("Contents", typeof(IEnumerable<RdlActor>), typeof(BagDialog), new PropertyMetadata(null, new PropertyChangedCallback(BagDialog.OnContentsPropertyChanged)));
		private static void OnContentsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)	
		{
			(obj as BagDialog).SetContents();
		}
			
		public BagDialog()
		{
			InitializeComponent();
		}

		private void SetCapacity()
		{
			if (this.Item != null)
			{
				int capacity = this.Item.Properties.GetValue<int>("Capacity");
				for (int i = 0; i < 16; i++)
				{
                    ItemSlot slot = this.FindName(String.Concat("ctlSlot", i)) as ItemSlot;
					if (slot != null)
					{
                        slot.OwnerID = this.Item.ID;
                        
						if (i > (capacity - 1))
							slot.Visibility = Visibility.Collapsed;
						else
							slot.Visibility = Visibility.Visible;
					}
				}
			}
		}

		private void SetContents()
		{
			if (this.Item != null && this.Contents != null && this.Contents.Count() > 0)
			{
				// Clear out the existing contents.
				for (int i = 0; i < 16; i++)
				{
					ItemSlot slot = this.FindName(String.Concat("ctlSlot", i)) as ItemSlot;
					if (slot != null)
					{
						slot.Item = null;
					}
				}

				// Add the new contents.
				int index = 0;
				foreach (var item in this.Contents)
				{
					ItemSlot slot = this.FindName(String.Concat("ctlSlot", index)) as ItemSlot;
					if (slot != null)
					{
						slot.Item = item;
					}
					index++;
				}
			}
			else
			{
				for (int i = 0; i < 16; i++)
				{
					ItemSlot slot = this.FindName(String.Concat("ctlSlot", i)) as ItemSlot;
					if (slot != null)
					{
						slot.Item = null;
					}
				}
			}
		}

		#region IDropContainer Members

		public void Drop(IDroppable item)
		{
			if (item is ItemSlot)
			{
				ItemSlot slot = (ItemSlot)item;
				if (slot.Item != null && this.Item != null)
				{
					switch (slot.SlotType)
					{
						case ItemSlotType.Item:
						case ItemSlotType.Equipment:
							// Find an open slot and drop the item.
							if (slot.EquipmentSlot != EquipmentSlot.None && slot.Item.Properties.GetValue<bool>("IsEquipped"))
							{
								// Unequip the item.
								slot.Item.OwnerID = this.Item.ID;
								slot.Item.Properties.SetValue("IsEquipped", false);
								ServerManager.Instance.SendCommand(new RdlCommand("UNEQUIP", slot.Item.ID, slot.EquipmentSlot, this.Item.ID));
							}
							else
							{
								ServerManager.Instance.SendCommand(new RdlCommand("GIVE", slot.Item.ID, this.Item.ID, 1));
							}
							break;
					}
				}
			}
		}

		#endregion
	}
}
