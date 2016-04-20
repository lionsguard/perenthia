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
	public partial class ItemListItem : UserControl, IActorListItem
	{
		public CommerceType CommerceType
		{
			get { return (CommerceType)GetValue(CommerceTypeProperty); }
			set { SetValue(CommerceTypeProperty, value); }
		}
		public static readonly DependencyProperty CommerceTypeProperty = DependencyProperty.Register("CommerceType", typeof(CommerceType), typeof(ItemListItem), new PropertyMetadata(CommerceType.None, new PropertyChangedCallback(ItemListItem.OnCommerceTypePropertyChanged)));
		private static void OnCommerceTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			//(obj as ItemListItem).SetButtons();
		}

		public RdlActor Actor
		{
			get { return (RdlActor)GetValue(ActorProperty); }
			set { SetValue(ActorProperty, value); }
		}
		public static readonly DependencyProperty ActorProperty = DependencyProperty.Register("Actor", typeof(RdlActor), typeof(ItemListItem), new PropertyMetadata(null, new PropertyChangedCallback(ItemListItem.OnActorPropertyChanged)));
		private static void OnActorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ItemListItem).Refresh();
		}

		public CurrencyType CurrencyType { get; set; }	

		public event ActionEventHandler Action = delegate { };

		private ItemDetails _details = null;

		public ItemListItem()
		{
			this.Loaded += new RoutedEventHandler(ItemListItem_Loaded);
			this.MouseLeftButtonDown += new MouseButtonEventHandler(ItemListItem_MouseLeftButtonDown);
			InitializeComponent();
		}

		void ItemListItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.Actor != null)
			{
				Logger.LogDebug("ItemListItem Mouse Down");
				this.BeginDrag(this, new BeginDragEventArgs { Droppable = this, MousePosition = e.GetPosition(null) });
			}
		}

		void ItemListItem_Loaded(object sender, RoutedEventArgs e)
		{
			this.CurrencyType = CurrencyType.None;
		}

		public void Refresh()
		{
			if (this.Actor != null)
			{
				MainImage.Source = Asset.GetImageSource(this.Actor.Properties.GetValue<string>("ImageUri"));
				NameLabel.Text = this.Actor.Name;

				int quantity = this.Actor.Properties.GetValue<int>("Quantity");
				if (quantity > 1)
				{
					QuantityLabel.Text = quantity.ToString();
					QuantityLabel.Visibility = Visibility.Visible;
				}
				else
				{
					QuantityLabel.Visibility = Visibility.Collapsed;
				}

				if (this.CommerceType == CommerceType.Buy || this.CommerceType == CommerceType.Sell)
				{
					Currency currency = new Currency(this.Actor.Properties.GetValue<int>("BuyCost"));
					if (currency.Value == 0) currency = new Currency(this.Actor.Properties.GetValue<int>("SellCost"));
					if (currency.Value > 0)
					{
						GoldLabel.Text = currency.Gold.ToString();
						SilverLabel.Text = currency.Silver.ToString();
						CopperLabel.Text = currency.Copper.ToString();
						this.CurrencyType = CurrencyType.Currency;
					}

					int emblem = this.Actor.Properties.GetValue<int>("EmblemBuyCost");
					if (emblem == 0) emblem = this.Actor.Properties.GetValue<int>("EmblemSellCost");
					if (emblem > 0)
					{
						EmblemLabel.Text = emblem.ToString();
						this.CurrencyType = CurrencyType.Emblem;
					}
				}

				if (this.Actor.Properties.GetValue<bool>("IsEquipped"))
				{
					VisualStateManager.GoToState(this, "Equipped", true);
				}
				else
				{
					VisualStateManager.GoToState(this, "Normal", true);
				}

				this.SetButtons();
			}
		}

		private void btnBuySellCurrency_Click(object sender, RoutedEventArgs e)
		{
			this.RaiseActionEvent();
		}

		private void btnBuySellEmblem_Click(object sender, RoutedEventArgs e)
		{
			this.RaiseActionEvent();
		}

		private void btnGetDrop_Click(object sender, RoutedEventArgs e)
		{
			this.RaiseActionEvent();
		}

		public void RaiseActionEvent()
		{
			string actionName = String.Empty;
			switch (this.CommerceType)
			{
				case CommerceType.Buy:
					actionName = Actions.Buy;
					break;
				case CommerceType.Sell:
					actionName = Actions.Sell;
					break;
				case CommerceType.Drop:
					actionName = Actions.Drop;
					break;
				default:
					actionName = Actions.Get;
					break;
			}
			if (!String.IsNullOrEmpty(actionName))
			{
				this.Action(this, new ActionEventArgs(actionName, this.Actor.ID, this.Actor.Name));
			}
		}

		private void SetButtons()
		{
			string buttonText = String.Empty;
			switch (this.CommerceType)
			{
				case CommerceType.Buy:
					buttonText = "Buy";
					break;
				case CommerceType.Sell:
					buttonText = "Sell";
					break;
				case CommerceType.Drop:
					buttonText = "Drop";
					break;
				default:
					// GET
					buttonText = "Get";
					break;
			}

			if (!String.IsNullOrEmpty(buttonText))
			{
				switch (this.CurrencyType)
				{
					case CurrencyType.Currency:
						CurrencyContainer.Visibility = Visibility.Visible;
						EmblemContainer.Visibility = Visibility.Collapsed;
						GetDropContainer.Visibility = Visibility.Collapsed;
						btnBuySellCurrency.Content = buttonText;
						break;
					case CurrencyType.Emblem:
						CurrencyContainer.Visibility = Visibility.Collapsed;
						GetDropContainer.Visibility = Visibility.Collapsed;
						EmblemContainer.Visibility = Visibility.Visible;
						btnBuySellEmblem.Content = buttonText;
						break;
					default:
						CurrencyContainer.Visibility = Visibility.Collapsed;
						EmblemContainer.Visibility = Visibility.Collapsed;
						GetDropContainer.Visibility = Visibility.Visible;
						btnGetDrop.Content = buttonText;
						break;
				}
			}
		}

		#region IDroppable Members

		public event BeginDragEventHandler BeginDrag = delegate { };

		public UIElement GetDragCursor()
		{
			Image img = new Image();
			img.Source = Asset.GetImageSource(this.Actor.Properties.GetValue<string>("ImageUri"));
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

		public static ItemListItem Create(RdlActor item)
		{
			ItemListItem listItem = new ItemListItem();
			listItem.Actor = item;
			return listItem;
		}

		private void NameLabel_MouseEnter(object sender, MouseEventArgs e)
		{
			_details = new ItemDetails();
			_details.Show(this.Actor, Game.Player);
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
	}
	public enum CommerceType
	{
		None,
		Buy,
		Sell,
		Get,
		Drop,
	}
	public enum CurrencyType
	{
		None,
		Currency,
		Emblem,
	}
}
