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
	public partial class AttributeSelection : UserControl
	{
		private static readonly int MaxAttributePoints = 40;

		public int AttributePoints { get; private set; }

		public static readonly DependencyProperty RaceProperty =
			DependencyProperty.Register("SelectedRace", typeof(Race), typeof(AttributeSelection), new PropertyMetadata(null, new PropertyChangedCallback(AttributeSelection.OnRacePropertyChanged)));
		public Race SelectedRace
		{
			get { return (Race)GetValue(RaceProperty); }
			set { SetValue(RaceProperty, value); }
		}

		public event AttributeChangedEventHandler AttributeChanged = delegate { };

		public AttributeSelection()
		{
			this.Loaded += new RoutedEventHandler(AttributeSelection_Loaded);
			InitializeComponent();
		}

		void AttributeSelection_Loaded(object sender, RoutedEventArgs e)
		{
			this.AttributePoints = MaxAttributePoints;
			this.ResetAttributeMinimums();
		}

		private static void OnRacePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as AttributeSelection).ResetAttributeMinimums();
		}

		private void OnError(object sender, NotificationEventArgs e)
		{
			MessageBox.Show(e.Message, "ERROR", MessageBoxButton.OK);
		}

		private void OnAttributeChanged(object sender, AttributeChangedEventArgs e)
		{
			AttributePanel pnl = sender as AttributePanel;

			// The attribute panels sends the requested newValue in the event args. Need to validate that the 
			// user has the points left to be able to make this change.
			int pointCost = this.GetPointCost(e.OldValue, e.NewValue);

			if ((this.AttributePoints + pointCost) < 0)
			{
				// Cancel the request.
				e.Cancel = true;
				e.Message = "Not enough Attribute Points to change this Attribute.";
			}
			else if ((this.AttributePoints + pointCost) > MaxAttributePoints)
			{
				// Cancel the request.
				e.Cancel = true;
				e.Message = "Not enough Attribute Points to increase this Attribute.";
			}
			else
			{
				// Accept the request, increment or decrement the points.
				this.AttributePoints += pointCost;
			}

			lblPoints.Text = this.AttributePoints.ToString();

			if (!e.Cancel)
			{
				// Bubble up the event.
				this.AttributeChanged(pnl, new AttributeChangedEventArgs(e.Name, e.OldValue, e.NewValue + pnl.AttributeMinimum));
			}
		}

		private void ResetAttributeMinimums()
		{
			if (this.SelectedRace != null)
			{
				this.ResetMinValue(statStr, this.SelectedRace.Strength);
				this.ResetMinValue(statDex, this.SelectedRace.Dexterity);
				this.ResetMinValue(statSta, this.SelectedRace.Stamina);
				this.ResetMinValue(statBea, this.SelectedRace.Beauty);
				this.ResetMinValue(statInt, this.SelectedRace.Intelligence);
				this.ResetMinValue(statPer, this.SelectedRace.Perception);
				this.ResetMinValue(statEnd, this.SelectedRace.Endurance);
				this.ResetMinValue(statAff, this.SelectedRace.Affinity);
			}
		}

		private void ResetMinValue(AttributePanel pnl, int minValue)
		{
			if (pnl.AttributeValue >= pnl.AttributeMinimum)
			{
				pnl.AttributeValue -= pnl.AttributeMinimum;
			}
			pnl.AttributeMinimum = minValue;
			pnl.AttributeValue += pnl.AttributeMinimum;
			pnl.RefreshView();
		}

		public int GetPointCost(int oldValue, int newValue)
		{
			// Point Cost	Attribute Score
			// 1			3
			// 4			4
			// 5			5
			// 6			6
			// 9			7
			int cost = 1;
			switch (newValue)
			{
				//case 3:
				//    if (oldValue > newValue) cost = 3;
				//    else cost = 1;
				//    break;
				//case 4:
				//    if (oldValue < newValue) cost = 3;
				//    else cost = 1;
				//    break;
				//case 5:
				//    cost = 1;
				//    break;
				case 6:
					if (oldValue > newValue) cost = 3;
					else cost = 1;
					break;
				case 7:
					cost = 3;
					break;
			}

			if (newValue > oldValue) return cost * -1;
			return cost;
		}
	}
}
