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
	public partial class AvatarDetails : UserControl
	{
		public AvatarDetails()
		{
			InitializeComponent();
		}

		public void Show(Avatar avatar)
		{
			if (avatar != null)
			{
				lblHealth.Text = String.Format("{0}/{1}", avatar.Body, avatar.BodyMax);
				lblWillpower.Text = String.Format("{0}/{1}", avatar.Mind, avatar.MindMax);
				lblXp.Text = String.Format("{0}/{1}", avatar.Experience, avatar.ExperienceMax);
				// TODO: Order
				lblLevel.Text = avatar.Level.ToString();
				if (avatar.Race != null)
				{
					lblRace.Text = avatar.Race.Name;
				}
				else
				{
					lblRace.Text = avatar.Properties.GetValue<string>("Race");
				}

				// Attributes
				this.SetAttribute(avatar.Strength, lblStr);
				this.SetAttribute(avatar.Dexterity, lblDex);
				this.SetAttribute(avatar.Stamina, lblSta);
				this.SetAttribute(avatar.Beauty, lblBea);
				this.SetAttribute(avatar.Intelligence, lblInt);
				this.SetAttribute(avatar.Perception, lblPer);
				this.SetAttribute(avatar.Endurance, lblEnd);
				this.SetAttribute(avatar.Affinity, lblAff);
			}
			this.Visibility = Visibility.Visible;
		}
		private void SetAttribute(int value, TextBlock label)
		{
			label.Text = value.ToString();
			if (value > 0)
			{
				label.Foreground = Brushes.PositiveBrush;
			}
			else
			{
				label.Foreground = Brushes.NegativeBrush;
			}
		}

		public void Hide()
		{
			this.Visibility = Visibility.Collapsed;
		}
	}
}
