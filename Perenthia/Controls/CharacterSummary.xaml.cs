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
using Perenthia.Models;

namespace Perenthia.Controls
{
	public partial class CharacterSummary : UserControl
	{
		public CharacterSummary()
		{
			this.Loaded += new RoutedEventHandler(CharacterSummary_Loaded);
			InitializeComponent();
		}

		private void CharacterSummary_Loaded(object sender, RoutedEventArgs e)
		{

		}

		public void SetAvatar(Avatar avatar)
		{
			if (avatar.Race != null)
			{
				imgRace.Source = Asset.GetImageSource(String.Format(Asset.AVATAR_FORMAT, avatar.Race.Name, avatar.Gender));
				lblRace.Text = avatar.Race.Name;
			}
			lblName.Text = avatar.Name;
			lblGender.Text = avatar.Gender.ToString();

			this.SetLabel(lblStr, avatar.Strength);
			this.SetLabel(lblDex, avatar.Dexterity);
			this.SetLabel(lblSta, avatar.Stamina);
			this.SetLabel(lblBea, avatar.Beauty);
			this.SetLabel(lblInt, avatar.Intelligence);
			this.SetLabel(lblPer, avatar.Perception);
			this.SetLabel(lblEnd, avatar.Endurance);
			this.SetLabel(lblAff, avatar.Affinity);

			spSkills.Children.Clear();
			foreach (var item in avatar.Skills.Values)
			{
				spSkills.Children.Add(new SkillNameValue(item));
			}
		}

		private void SetLabel(TextBlock lbl, int value)
		{
			lbl.Text = value.ToString();
			switch (value)
			{	
				case 1:
					lbl.Foreground = Brushes.StatTerribleBrush;
					break;
				case 2:
					lbl.Foreground = Brushes.StatBadBrush;
					break;
				case 3:
					lbl.Foreground = Brushes.StatPoorBrush;
					break;
				case 4:
					lbl.Foreground = Brushes.StatBelowAverageBrush;
					break;
				case 5:
					lbl.Foreground = Brushes.StatAverageBrush;
					break;
				case 6:
					lbl.Foreground = Brushes.StatAboveAverageBrush;
					break;
				case 7:
					lbl.Foreground = Brushes.StatExcellentBrush;
					break;
				case 8:
					lbl.Foreground = Brushes.StatSuperbBrush;
					break;
				default:
					lbl.Foreground = Brushes.StatEmptyBrush;
					break;
			}
		}
	}
}
