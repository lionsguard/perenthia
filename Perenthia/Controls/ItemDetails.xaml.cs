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
	public partial class ItemDetails : UserControl
	{
		public ItemDetails()
		{
			InitializeComponent();
		}

		public void Show(RdlActor item, Avatar viewer)
		{
			this.SetDetails(item, viewer);
			this.Visibility = Visibility.Visible;
		}

		public void Hide()
		{
			this.Visibility = Visibility.Collapsed;
		}

		public void SetDetails(RdlActor item, Avatar viewer)
		{
			if (item != null && viewer != null)
			{
				// Skill Name
				// Skill Value
				string skill = item.Properties.GetValue<string>("Skill");
				int skillValue = item.Properties.GetValue<int>("SkillLevelRequiredToEquip");
				if (!String.IsNullOrEmpty(skill))
				{
					lblSkillName.Text = String.Format("{0} ({1})", skill, skillValue);

					string skillKey = String.Concat("Skill_", skill);
					int viewerSkillValue = 0;
					if (viewer.Skills.ContainsKey(skillKey))
					{
						viewerSkillValue = viewer.Skills[skillKey].Value;
					}
					lblSkillValue.Text = viewerSkillValue.ToString();
					if (viewerSkillValue >= skillValue)
					{
						lblSkillName.Foreground = Brushes.PositiveBrush;
						lblSkillValue.Foreground = Brushes.PositiveBrush;
					}
					else
					{
						lblSkillName.Foreground = Brushes.NegativeBrush;
						lblSkillValue.Foreground = Brushes.NegativeBrush;
					}
				}
				else
				{
					lblSkillName.Text = "None";
					lblSkillName.Foreground = Brushes.TextAltBrush;

					lblSkillValue.Text = "0";
					lblSkillValue.Foreground = Brushes.TextAltBrush;
				}

				// Equip Location
				lblEquipLocation.Text = item.Properties.GetValue<string>("EquipLocation");
				
				// Durability
				lblDurability.Text = String.Format("{0}/{1}", 
					item.Properties.GetValue<int>("Durability"), 
					item.Properties.GetValue<int>("DurabilityMax"));

				// Protection
				lblProtection.Text = item.Properties.GetValue<int>("Protection").ToString();

				// Power
				lblPower.Text = item.Properties.GetValue<int>("Power").ToString();

				// Affects
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Strength"), lblStr);
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Dexterity"), lblDex);
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Stamina"), lblSta);
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Beauty"), lblBea);
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Intelligence"), lblInt);
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Perception"), lblPer);
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Endurance"), lblEnd);
				this.SetAttribute(item.Properties.GetValue<int>("Attr_Affinity"), lblAff);

				// Notes
				lblNotes.Text = item.Description;
			}
		}
		private void SetAttribute(int value, TextBlock label)
		{
			label.Text = value.ToString();
			if (value > 0)
			{
				label.Foreground = Brushes.PositiveBrush;
			}
			else if (value < 0)
			{
				label.Foreground = Brushes.NegativeBrush;
			}
			else
			{
				label.Foreground = Brushes.TextAltBrush;
			}
		}
	}
}
