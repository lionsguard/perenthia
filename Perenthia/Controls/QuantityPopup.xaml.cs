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

namespace Perenthia.Controls
{
	public partial class QuantityPopup : UserControl
	{
		public bool IsSell
		{	
			get { return (bool)GetValue(IsSellProperty); }
			set { SetValue(IsSellProperty, value); }
		}
		public static readonly DependencyProperty IsSellProperty = DependencyProperty.Register("IsSell", typeof(bool), typeof(QuantityPopup), new PropertyMetadata(true, new PropertyChangedCallback(QuantityPopup.OnIsSellPropertyChanged)));
		private static void OnIsSellPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as QuantityPopup).SetSell();
		}

		public event QuantitySelectedEventHandler QuantitySelected = delegate { };

		private int _maxQuantity = 0;
		private int _itemId = 0;

		public QuantityPopup()
		{
			InitializeComponent();
		}

		private void btnDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.ChangeQuantity(-1);
		}

		private void btnUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.ChangeQuantity(1);
		}

		private void SetSell()
		{
			if (!this.IsSell)
			{
				btnSell.Content = "BUY";
				btnSellAll.Content = "BUY ALL";
			}
		}

		private void ChangeQuantity(int value)
		{
			int result;
			if (Int32.TryParse(txtQuantity.Text, out result))
			{
				result += value;
				if (result > _maxQuantity) result = _maxQuantity;
				if (result == 0) result = 1;
				txtQuantity.Text = result.ToString();
			}
		}

		private void btnSell_Click(object sender, RoutedEventArgs e)
		{
			this.RaiseSelectedEvent();
		}

		private void btnSellAll_Click(object sender, RoutedEventArgs e)
		{
			txtQuantity.Text = _maxQuantity.ToString();
			this.RaiseSelectedEvent();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void RaiseSelectedEvent()
		{
			int result = 0;
			Int32.TryParse(txtQuantity.Text, out result);

			if (result > _maxQuantity) result = _maxQuantity;

			this.QuantitySelected(this, new QuantitySelectedEventArgs(_itemId, result));
			this.Close();
		}

		public void Show(int itemId, int quantityMax)
		{
			_itemId = itemId;
			_maxQuantity = quantityMax;
			this.Visibility = Visibility.Visible;
		}

		public void Close()
		{
			this.Visibility = Visibility.Collapsed;
		}
	}

	public delegate void QuantitySelectedEventHandler(object sender, QuantitySelectedEventArgs e);
	public class QuantitySelectedEventArgs : EventArgs
	{
		public int ItemID { get; set; }
		public int Quantity { get; set; }

		public QuantitySelectedEventArgs(int itemId, int quantity)
		{
			this.ItemID = itemId;
			this.Quantity = quantity;
		}
	}
}
