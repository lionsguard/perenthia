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
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class AffectIcon : UserControl
	{
		public Property Affect
		{	
			get { return (Property)GetValue(AffectProperty); }
			set { SetValue(AffectProperty, value); }
		}
		public static readonly DependencyProperty AffectProperty = DependencyProperty.Register("Affect", typeof(Property), typeof(AffectIcon), new PropertyMetadata(null, new PropertyChangedCallback(AffectIcon.OnAffectPropertyChanged)));
		private static void OnAffectPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as AffectIcon).Refresh();
		}

		public Avatar Owner { get; set; }

		public AffectIcon()
		{
			this.Loaded += new RoutedEventHandler(AffectIcon_Loaded);
			this.MouseEnter += new MouseEventHandler(AffectIcon_MouseEnter);
			this.MouseLeave += new MouseEventHandler(AffectIcon_MouseLeave);
			InitializeComponent();
		}
		public AffectIcon(Avatar owner)
			: this()
		{
			this.Owner = owner;
		}

		void AffectIcon_Loaded(object sender, RoutedEventArgs e)
		{
		}

		void AffectIcon_MouseLeave(object sender, MouseEventArgs e)
		{
		}

		void AffectIcon_MouseEnter(object sender, MouseEventArgs e)
		{
			RdlActor item = this.GetItem();
			TimeSpan duration = new TimeSpan(Convert.ToInt64(this.Affect.Value));
			TimeSpan now = new TimeSpan(DateTime.Now.ToUniversalTime().Ticks);
			TimeSpan remainder = now.Subtract(duration);

			string minutes = String.Empty;
			if (remainder.TotalMinutes > 1)
			{
				minutes = String.Format("{0} minutes remaining.", remainder.TotalMinutes);
			}
			else if (remainder.TotalMinutes < 1)
			{
				minutes = String.Format("{0} seconds remaining.", remainder.TotalSeconds);
			}
			else
			{
				minutes = "1 minute remaining.";
			}

			int power = item.Properties.GetValue<int>("AffectPower");
			string sign = "+";
			if (power < 0) sign = "-";

			ToolTipService.SetToolTip(this, String.Format("{0} ({1}{2} {3}) {4}", 
				this.Affect.Name.Replace("Affect_", String.Empty),
				sign,
				power,
				item.Properties.GetValue<string>("AffectType"),
				minutes));
		}

		public void Refresh()
		{
			if (this.Affect != null && this.Owner != null)
			{
				// Find the item assoiated with this affect.
				RdlActor item = this.GetItem();
				if (item != null)
				{
					ToolTipService.SetToolTip(this, item.Name.Replace("Affect_", String.Empty));
					imgMain.Source = ImageManager.GetImageSource(item.Properties.GetValue<string>("ImageUri"));
				}
			}
		}

		public RdlActor GetItem()
		{
			string name = this.Affect.Name.Replace("Affect_", String.Empty);
			return this.Owner.Inventory.Where(i => i.Name == name).FirstOrDefault();
		}
	}
}
