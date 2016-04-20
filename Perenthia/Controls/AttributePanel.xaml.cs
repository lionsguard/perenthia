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
	public partial class AttributePanel : UserControl
	{
		public event AttributeChangedEventHandler AttributeChanged = delegate { };
		public event NotificationEventHandler Error = delegate { };

		public int AttributeValue { get; set; }
		public int AttributeMinimum { get; set; }	// For race presets

		public string StatName
		{
			get { return lblName.Text; }
			set { lblName.Text = value; }
		}

		public string Description
		{
			get { return lblDesc.Text; }
			set { lblDesc.Text = value; }
		}

		public AttributePanel()
		{
			this.Loaded += new RoutedEventHandler(AttributePanel_Loaded);
			InitializeComponent();
		}

		void AttributePanel_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void btnPlus_Click(object sender, RoutedEventArgs e)
		{
			int value = this.AttributeValue - this.AttributeMinimum;
			value++;

			if (value > 7)
			{
				// You can not increase this Attribute any further.
				this.Error(this, new NotificationEventArgs("You can not increase this Attribute any further."));
				return;
			}

			this.RaiseAttributeChangedEvent(value);
		}

		private void btnMinus_Click(object sender, RoutedEventArgs e)
		{
			int value = this.AttributeValue - this.AttributeMinimum;
			value--;

			if (value < 0)
			{
				// You can not reduce this attribute any further.
				this.Error(this, new NotificationEventArgs("You can not reduce this Attribute any further."));
				return;
			}

			this.RaiseAttributeChangedEvent(value);
		}

		private void RaiseAttributeChangedEvent(int newValue)
		{
			AttributeChangedEventArgs args = new AttributeChangedEventArgs(this.StatName, this.AttributeValue - this.AttributeMinimum, newValue);
			args.MinValue = this.AttributeMinimum;
			this.AttributeChanged(this, args);

			if (!args.Cancel)
			{
				this.AttributeValue = newValue + this.AttributeMinimum;

				// Set the block according to the value of the attribute.
				this.RefreshView();
			}
			else
			{
				this.Error(this, new NotificationEventArgs(args.Message));
			}
		}

		public void RefreshView()
		{
			statTerrible.Fill = Brushes.StatEmptyBrush;
			statBad.Fill = Brushes.StatEmptyBrush;
			statPoor.Fill = Brushes.StatEmptyBrush;
			statBelowAverage.Fill = Brushes.StatEmptyBrush;
			statAverage.Fill = Brushes.StatEmptyBrush;
			statAboveAverage.Fill = Brushes.StatEmptyBrush;
			statExcellent.Fill = Brushes.StatEmptyBrush;
			statSuperb.Fill = Brushes.StatEmptyBrush;

			lblValue.Foreground = Brushes.StatEmptyBrush;
			lblValue.Text = String.Format("({0})", this.AttributeValue);

			if (this.AttributeValue >= 1)
			{
				statTerrible.Fill = Brushes.StatTerribleBrush;
				lblValue.Foreground = Brushes.StatTerribleBrush;
			}
			if (this.AttributeValue >= 2)
			{
				statBad.Fill = Brushes.StatBadBrush;
				lblValue.Foreground = Brushes.StatBadBrush;
			}
			if (this.AttributeValue >= 3)
			{
				statPoor.Fill = Brushes.StatPoorBrush;
				lblValue.Foreground = Brushes.StatPoorBrush;
			}
			if (this.AttributeValue >= 4)
			{
				statBelowAverage.Fill = Brushes.StatBelowAverageBrush;
				lblValue.Foreground = Brushes.StatBelowAverageBrush;
			}
			if (this.AttributeValue >= 5)
			{
				statAverage.Fill = Brushes.StatAverageBrush;
				lblValue.Foreground = Brushes.StatAverageBrush;
			}
			if (this.AttributeValue >= 6)
			{
				statAboveAverage.Fill = Brushes.StatAboveAverageBrush;
				lblValue.Foreground = Brushes.StatAboveAverageBrush;
			}
			if (this.AttributeValue >= 7)
			{
				statExcellent.Fill = Brushes.StatExcellentBrush;
				lblValue.Foreground = Brushes.StatExcellentBrush;
			}
			if (this.AttributeValue >= 8)
			{
				statSuperb.Fill = Brushes.StatSuperbBrush;
				lblValue.Foreground = Brushes.StatSuperbBrush;
			}
		}
	}

	public delegate void AttributeChangedEventHandler(object sender, AttributeChangedEventArgs e);
	public class AttributeChangedEventArgs : EventArgs
	{
		public string Name { get; set; }	
		public int OldValue { get; set; }
		public int NewValue { get; set; }
		public int MinValue { get; set; }
		public string Message { get; set; }	
		public bool Cancel { get; set; }	

		public AttributeChangedEventArgs(string name, int oldValue, int newValue)
		{
			this.Name = name;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
	}
}
