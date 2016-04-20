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
	public partial class AttributesPanel : UserControl
	{
		public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register("Strength", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Strength
		{
			get { return (int)this.GetValue(StrengthProperty); }
			set { this.SetValue(StrengthProperty, value); }
		}
		public static readonly DependencyProperty DexterityProperty = DependencyProperty.Register("Dexterity", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Dexterity
		{
			get { return (int)this.GetValue(DexterityProperty); }
			set { this.SetValue(DexterityProperty, value); }
		}
		public static readonly DependencyProperty StaminaProperty = DependencyProperty.Register("Stamina", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Stamina
		{
			get { return (int)this.GetValue(StaminaProperty); }
			set { this.SetValue(StaminaProperty, value); }
		}
		public static readonly DependencyProperty BeautyProperty = DependencyProperty.Register("Beauty", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Beauty
		{
			get { return (int)this.GetValue(BeautyProperty); }
			set { this.SetValue(BeautyProperty, value); }
		}
		public static readonly DependencyProperty IntelligenceProperty = DependencyProperty.Register("Intelligence", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Intelligence
		{
			get { return (int)this.GetValue(IntelligenceProperty); }
			set { this.SetValue(IntelligenceProperty, value); }
		}
		public static readonly DependencyProperty PerceptionProperty = DependencyProperty.Register("Perception", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Perception
		{
			get { return (int)this.GetValue(PerceptionProperty); }
			set { this.SetValue(PerceptionProperty, value); }
		}
		public static readonly DependencyProperty EnduranceProperty = DependencyProperty.Register("Endurance", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Endurance
		{
			get { return (int)this.GetValue(EnduranceProperty); }
			set { this.SetValue(EnduranceProperty, value); }
		}
		public static readonly DependencyProperty AffinityProperty = DependencyProperty.Register("Affinity", typeof(int), typeof(AttributesPanel), new PropertyMetadata(0, new PropertyChangedCallback(AttributesPanel.OnAttributePropertyChanged)));
		public int Affinity
		{
			get { return (int)this.GetValue(AffinityProperty); }
			set { this.SetValue(AffinityProperty, value); }
		}

		public AttributesPanel()
		{
			this.Loaded += new RoutedEventHandler(AttributesPanel_Loaded);
			InitializeComponent();
		}

		void AttributesPanel_Loaded(object sender, RoutedEventArgs e)
		{
			this.SetLabels();
		}

		private static void OnAttributePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AttributesPanel attr = d as AttributesPanel;
			attr.SetLabels();
		}

		private void SetLabels()
		{
			if (_contentLoaded)
			{
				this.SetLabel(lblAff, this.Affinity);
				this.SetLabel(lblBea, this.Beauty);
				this.SetLabel(lblDex, this.Dexterity);
				this.SetLabel(lblEnd, this.Endurance);
				this.SetLabel(lblInt, this.Intelligence);
				this.SetLabel(lblPer, this.Perception);
				this.SetLabel(lblSta, this.Stamina);
				this.SetLabel(lblStr, this.Strength);
			}
		}

		private void SetLabel(TextBlock lbl, int value)
		{
			lbl.Text = this.GetValueName(value);
			lbl.Foreground = this.GetValueBrush(value);
		}

		private string GetValueName(int value)
		{
			if (value > 0)
			{
				return String.Concat("+", value);
			}
			return value.ToString();
		}

		private Brush GetValueBrush(int value)
		{
			if (value > 0)
			{
				return Brushes.PositiveBrush;
			}
			else if (value < 0)
			{
				return Brushes.NegativeBrush;
			}
			return Brushes.TextAltBrush;
		}
	}
}
