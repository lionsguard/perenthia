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

using Lionsguard;
using Perenthia.Controls;
using Radiance;
using Radiance.Markup;
using Perenthia.Models;

namespace Perenthia.Dialogs
{
	public partial class CharacterSheetDialog : UserControl
	{
		public bool ShowUntrainedSkills { get; set; }	

		public Avatar Avatar
		{
			get { return (Avatar)GetValue(AvatarProperty); }
			set { SetValue(AvatarProperty, value); }
		}
		public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register("Avatar", typeof(Avatar), typeof(CharacterSheetDialog), new PropertyMetadata(null, new PropertyChangedCallback(CharacterSheetDialog.OnAvatarPropertyChanged)));
		private static void OnAvatarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as CharacterSheetDialog).Refresh();
		}

		public event SkillChangedEventHandler SkillChanged = delegate { };

		public CharacterSheetDialog()
		{
			this.Loaded += new RoutedEventHandler(CharacterSheetDialog_Loaded);
			InitializeComponent();
		}

		void CharacterSheetDialog_Loaded(object sender, RoutedEventArgs e)
		{
			this.ShowUntrainedSkills = true;
			ctlSkills.EnableButtons = false;
			ctlSkills.ShowSkillPoints = false;
			ctlSkills.SkillChanged += new Perenthia.Controls.SkillChangedEventHandler(ctlSkills_SkillChanged);
		}

		void ctlSkills_SkillChanged(object sender, Perenthia.Controls.SkillChangedEventArgs e)
		{
			this.Avatar.Skills[e.Skill.Name].Value = e.NewValue;
			this.SkillChanged(this, e);
		}

		private void lnkHideSkills_Click(object sender, RoutedEventArgs e)
		{
			this.ShowUntrainedSkills = !this.ShowUntrainedSkills;
			if (!this.ShowUntrainedSkills) lnkHideSkills.Content = "Show Untrained Skills";
			else lnkHideSkills.Content = "Hide Untrained Skills";

			ctlSkills.ShowUntrainedSkills = this.ShowUntrainedSkills;
		}

		public void Refresh()
		{
			if (this.Avatar != null)
			{
				// Avatar Properties
				ctlPlayer.SetProperties(this.Avatar);

				// Xp
				ctlXp.Maximum = this.Avatar.ExperienceMax;
				ctlXp.Value = this.Avatar.Experience;

				// Skills
				// Set the MaxSkillValue to the highest skill value.
				ctlSkills.MaximumSkillValue = (from s in this.Avatar.Skills.Values select s.Value).Max() + this.Avatar.SkillPoints;
				//ctlSkills.EnableButtons = false;
				ctlSkills.SkillsSource = this.Avatar.Skills.Values;
				//if (this.Avatar.SkillPoints <= 0)
				//{
				//	ctlSkills.EnableButtons = false;
				//}
				ctlSkills.Refresh();

				// Equipment
				this.RefreshEquipmentSlots();
				var items = this.Avatar.Inventory.GetEquipment();
				if (items != null && items.Count > 0)
				{
					foreach (var item in items)
					{
						string equipLocation = item.Properties.GetValue<string>("EquipLocation");
						if (!String.IsNullOrEmpty(equipLocation))
						{
							ItemSlot slot = null;
							if (equipLocation.Equals("Ear") || equipLocation.Equals("Weapon") || equipLocation.Equals("Finger"))
							{
								// Use the first slot.
								int id1 = this.Avatar.Properties.GetValue<int>(String.Concat("Equipment_", equipLocation, 1));
								int id2 = this.Avatar.Properties.GetValue<int>(String.Concat("Equipment_", equipLocation, 2));

								if (item.ID == id1)
								{
									slot = this.FindName(String.Concat("ctl", equipLocation, "0")) as ItemSlot;
								}
								else if (item.ID == id2)
								{
									slot = this.FindName(String.Concat("ctl", equipLocation, "1")) as ItemSlot;
								}
							}
							else
							{
								slot = this.FindName(String.Concat("ctl", equipLocation)) as ItemSlot;
							}

							if (slot != null)
							{
								// Load the image for the item
								slot.Item = item;
							}
						}
					}
				}
			}
		}

		private void RefreshEquipmentSlots()
		{
			ctlArms.Item = null;
			ctlBack.Item = null;
			ctlChest.Item = null;
			ctlEar0.Item = null;
			ctlEar1.Item = null;
			ctLegs.Item = null;
			ctlFeet.Item = null;
			ctlFinger0.Item = null;
			ctlFinger1.Item = null;
			ctlHands.Item = null;
			ctlHat.Item = null;
			ctlHead.Item = null;
			ctlLight.Item = null;
			ctlNeck.Item = null;
			ctlNecklace.Item = null;
			ctlNose.Item = null;
			ctlPants.Item = null;
			ctlRobe.Item = null;
			ctlShield.Item = null;
			ctlShirt.Item = null;
			ctlShoulders.Item = null;
			ctlWaist.Item = null;
			ctlWeapon0.Item = null;
			ctlWeapon1.Item = null;
			ctlWrists.Item = null;
		}
	}
}
