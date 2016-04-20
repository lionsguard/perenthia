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
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class CommerceActionWindow : UserControl, IActionWindow
	{
		public Avatar Target
		{
			get { return (Avatar)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Avatar), typeof(CommerceActionWindow), new PropertyMetadata(null, new PropertyChangedCallback(CommerceActionWindow.OnTargetPropertyChanged)));
		private static void OnTargetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as CommerceActionWindow).Refresh();
		}

		public Avatar Player
		{
			get { return (Avatar)GetValue(PlayerProperty); }
			set { SetValue(PlayerProperty, value); }
		}
		public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register("Player", typeof(Avatar), typeof(CommerceActionWindow), new PropertyMetadata(null, new PropertyChangedCallback(CommerceActionWindow.OnPlayerPropertyChanged)));
		private static void OnPlayerPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as CommerceActionWindow).Refresh();	
		}

		public int SelectedQuantity
		{	
			get { return (int)GetValue(SelectedQuantityProperty); }
			set { SetValue(SelectedQuantityProperty, value); }
		}
		public static readonly DependencyProperty SelectedQuantityProperty = DependencyProperty.Register("SelectedQuantity", typeof(int), typeof(CommerceActionWindow), new PropertyMetadata(1, new PropertyChangedCallback(CommerceActionWindow.OnSelectedQuantityPropertyChanged)));
		private static void OnSelectedQuantityPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
		}

		private ActionEventArgs _args = null;

		public CommerceActionWindow()
		{
			this.Loaded += new RoutedEventHandler(CommerceActionWindow_Loaded);
			InitializeComponent();
		}

		void CommerceActionWindow_Loaded(object sender, RoutedEventArgs e)
		{
		}

		public void ShowLoading()
		{
			ctlLoading.Visibility = Visibility.Visible;
		}

		public void HideLoading()
		{
			ctlLoading.Visibility = Visibility.Collapsed;
		}

		#region IActionWindow Members
		public Window ParentWindow { get; set; }

		public event ActionEventHandler Action = delegate { };

		public void Load(ActionEventArgs args)
		{
		}
		#endregion

		public void Refresh()
		{
			if (this.Target != null)
			{
				lstGoods.Children.Clear();
				foreach (var item in this.Target.Inventory.GetContents().Where(i => !ActorHelper.HasFlag(i, "NoSell")))
				{
					// Ensure that the buy prices have been set.
					ItemHelper.EnsureBuyCost(item, this.Target.Properties.GetValue<double>("MarkupPercentage"));

					ItemListItem listItem = ItemListItem.Create(item);
					listItem.CommerceType = CommerceType.Buy;
					listItem.Action += new ActionEventHandler(OnListItemAction);
					listItem.Refresh();
					lstGoods.Children.Add(listItem);
				}
				this.HideLoading();
			}

			if (this.Player != null)
			{
				lstInventory.Children.Clear();
				foreach (var item in this.Player.Inventory.GetContents().Where(i => !ActorHelper.HasFlag(i, "NoSell")))
				{
					// Ensure that the sell prices have been set.
					if (this.Target != null)
					{
						ItemHelper.EnsureSellCost(item, this.Target.Properties.GetValue<double>("MarkdownPercentage"));
					}

					ItemListItem listItem = ItemListItem.Create(item);
					listItem.CommerceType = CommerceType.Sell;
					listItem.Action += new ActionEventHandler(OnListItemAction);
					listItem.Refresh();
					lstInventory.Children.Add(listItem);
				}
			}
		}

		private void OnListItemAction(object sender, ActionEventArgs e)
		{
			// Find the item in the either the player or the target's inventory.
			var item = this.Player.Inventory.GetContents().Where(i => i.ID == (int)e.ActorAlias).FirstOrDefault();
			if (item == null)
			{
				item = this.Target.Inventory.GetContents().Where(i => i.ID == (int)e.ActorAlias).FirstOrDefault();
			}

			_args = e;

			this.SelectedQuantity = 1;
			if (item != null)
			{
				int quantity = item.Properties.GetValue<int>("Quantity");
				bool isSell = (e.ActionName == Actions.Sell);
				if (!isSell) quantity = 20;
				if (quantity > 1)
				{
					// Present the quantity window before sending the action.
					QuantityPopupManager.ShowQuantity("Commerce", isSell, item.ID, quantity, new QuantitySelectedEventHandler(this.OnQuantitySelected));
					return;
				}
			}
			this.RaiseActionEvent();
		}

		private void OnQuantitySelected(object sender, QuantitySelectedEventArgs e)
		{
			this.SelectedQuantity = e.Quantity;
			this.RaiseActionEvent();
		}

		private void RaiseActionEvent()
		{
			// Append the quantity.
			_args.Args.Add(this.SelectedQuantity);

			// Append the Target ID, used for SELL
			_args.Args.Add(this.Target.ID);

			this.Action(this, _args);
		}
	}
}
