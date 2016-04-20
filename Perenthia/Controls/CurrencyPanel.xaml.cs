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
	public partial class CurrencyPanel : UserControl
	{
		public Currency Currency
		{
			get { return (Currency)GetValue(CurrencyProperty); }
			set { SetValue(CurrencyProperty, value); }
		}
		public static readonly DependencyProperty CurrencyProperty = DependencyProperty.Register("Currency", typeof(Currency), typeof(CurrencyPanel), new PropertyMetadata(new Currency(0), new PropertyChangedCallback(CurrencyPanel.OnCurrencyPropertyChanged)));
		private static void OnCurrencyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as CurrencyPanel).BindCurrency();
		}

		public int Emblem
		{	
			get { return (int)GetValue(EmblemProperty); }
			set { SetValue(EmblemProperty, value); }
		}
		public static readonly DependencyProperty EmblemProperty = DependencyProperty.Register("Emblem", typeof(int), typeof(CurrencyPanel), new PropertyMetadata(0, new PropertyChangedCallback(CurrencyPanel.OnEmblemPropertyChanged)));
		private static void OnEmblemPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as CurrencyPanel).EmblemLabel.Text = e.NewValue.ToString();
		}
			
		public CurrencyPanel()
		{
			InitializeComponent();
		}

		private void BindCurrency()
		{
			if (this.Currency != null)
			{
				GoldLabel.Text = this.Currency.Gold.ToString();
				SilverLabel.Text = this.Currency.Silver.ToString();
				CopperLabel.Text = this.Currency.Copper.ToString();
			}
		}
	}
}
