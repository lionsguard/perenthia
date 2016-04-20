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
	public partial class PropertyList : UserControl
	{
		public RdlPropertyCollection Properties
		{
			get { return (RdlPropertyCollection)GetValue(PropertiesProperty); }
			set { SetValue(PropertiesProperty, value); }
		}
		public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(RdlPropertyCollection), typeof(PropertyList), new PropertyMetadata(null, new PropertyChangedCallback(PropertyList.OnPropertiesPropertyChanged)));
		private static void OnPropertiesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as PropertyList).RefreshProperties();
		}

		private RdlActor Actor { get; set; }

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public PropertyList()
		{
			InitializeComponent();
		}

		public void SetProperties(RdlActor actor)
		{
			this.Actor = actor;
			this.Properties = actor.Properties;
		}

		private void RefreshProperties()
		{
			if (this.Properties != null)
			{
				ctlItems.Children.Clear();
				foreach (var p in this.Properties)
				{
					PropertyListItem item = new PropertyListItem();
					item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
					item.SetProperty(p);
					ctlItems.Children.Add(item);
				}
			}
		}

		private void item_PropertyChanged(PropertyChangedEventArgs e)
		{
			this.PropertyChanged(e);
			if (this.Actor != null)
			{
				this.Actor.Properties.SetValue(e.Property.Name, e.Property.Value);
			}
		}
	}
}
