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
	public partial class SpellbookDialog : UserControl,IDropContainer
	{
		public SpellbookDialog()
		{
			InitializeComponent();
		}

		public void Clear()
		{
			for (int i = 0; i < 64; i++)
			{
                ItemSlot slot = this.FindName(String.Concat("ctlSlot", i)) as ItemSlot;
				if (slot != null)
				{
					slot.Item = null;
				}
			}
		}

		#region IDropContainer Members

		public void Drop(IDroppable item)
		{
			if (item is ItemSlot)
			{
				ItemSlot slot = (ItemSlot)item;
				if (slot.Item != null)
				{
					switch (slot.SlotType)
					{	
						case ItemSlotType.Item:
						case ItemSlotType.Equipment:
							// If the destination item is not of equipLocation spell then it can not be dropped here.
							if (!slot.Item.Properties.GetValue<string>("EquipLocation").Equals(EquipLocation.Spell.ToString(), StringComparison.InvariantCultureIgnoreCase))
							{
								// Can not drop this item in the spellbook, it is not a spell.
								MessageBox.Show("You can not drop this item here, it is not a Spell!", "Invalid Spellbook Item", MessageBoxButton.OK);
							}
							else
							{
								slot.Item.Properties.SetValue("IsEquipped", true);
								ServerManager.Instance.SendCommand(new RdlCommand("EQUIP", slot.Item.ID, EquipmentSlot.None));
							}
							break;
					}
				}
			}
		}

		#endregion
	}
}
