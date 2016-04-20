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
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class InventoryActionWindow : UserControl, IActionWindow
	{
		public Avatar Target
		{	
			get { return (Avatar)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Avatar), typeof(InventoryActionWindow), new PropertyMetadata(null, new PropertyChangedCallback(InventoryActionWindow.OnTargetPropertyChanged)));
		private static void OnTargetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as InventoryActionWindow).Refresh();
		}

		public InventoryActionWindow()
		{
			InitializeComponent();
		}

		public void Refresh()
		{
			if (this.Target != null)
			{
				lblName.Text = String.Concat("Corpse of ", this.Target.Name.A(this.Target.Properties.GetValue<bool>("HasProperName")));

				lstInventory.Children.Clear();
				foreach (var item in this.Target.Inventory.GetContents().Where(i => !ActorHelper.HasFlag(i, "NoSell")
					&& i.Properties.GetValue<bool>("IsInventoryItem")))
				{
					ItemListItem listItem = ItemListItem.Create(item);
					listItem.CommerceType = CommerceType.Get;
					listItem.Action += new ActionEventHandler(OnListItemAction);
					listItem.Refresh();
					lstInventory.Children.Add(listItem);
				}
			}
		}

		private void OnListItemAction(object sender, ActionEventArgs e)
		{
			int quantity = (sender as ItemListItem).Actor.Properties.GetValue<int>("Quantity");
			if (quantity == 0) quantity = 1;

			// Append the quantity.
			e.Args.Add(quantity);

			this.Action(sender, e);
		}

		#region IActionWindow Members
		public Window ParentWindow { get; set; }

		public event ActionEventHandler Action = delegate { };

		public void Load(ActionEventArgs args)
		{
		}

		#endregion

		private void btnLootAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (var listItem in lstInventory.Children)
			{
				if (listItem is ItemListItem)
				{
					(listItem as ItemListItem).RaiseActionEvent();
				}
			}

			if (this.ParentWindow != null) this.ParentWindow.Close();
		}
	}
}
