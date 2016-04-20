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
	public partial class PropertyListItem : UserControl
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		private RdlProperty Property { get; set; }	

		public PropertyListItem()
		{
			InitializeComponent();
		}

		public void SetProperty(RdlProperty property)
		{
			this.Property = property;
			lblName.Text = property.Name;
			this.SetValue(property.Value);
		}

		private void SetValue(object value)
		{
			if (value != null)
			{
				txtValue.Visibility = cboValue.Visibility = cbxValue.Visibility = Visibility.Collapsed;
				// Analyze the value to see what type it is and cast it as such.
				if (Boolean.TrueString.Equals(value) || Boolean.FalseString.Equals(value))
				{
					cbxValue.Visibility = Visibility.Visible;
					cbxValue.IsChecked = Boolean.Parse(value.ToString());
				}
				else
				{
					txtValue.Visibility = Visibility.Visible;
					txtValue.Text = value.ToString();
				}
			}
		}

		private void SetPropertyValue(object newValue)
		{
			object oldValue = this.Property.Value;
			this.Property.Value = newValue;
			this.PropertyChanged(new PropertyChangedEventArgs { Property = this.Property, OldValue = oldValue, NewValue = newValue });
		}

		private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.SetPropertyValue(txtValue.Text);
		}

		private void cbxValue_Checked(object sender, RoutedEventArgs e)
		{
			this.SetPropertyValue(true);
		}

		private void cbxValue_Unchecked(object sender, RoutedEventArgs e)
		{
			this.SetPropertyValue(false);
		}

		private void cboValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.SetPropertyValue(cboValue.SelectedItem);
		}
	}

	public delegate void PropertyChangedEventHandler(PropertyChangedEventArgs e);
	public class PropertyChangedEventArgs : EventArgs
	{
		public RdlProperty Property { get; set; }
		public object OldValue { get; set; }
		public object NewValue { get; set; }	
	}
}
