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
using Radiance;

namespace Perenthia.Controls
{
	public partial class RaceSelection : UserControl
	{
		public event EventHandler RaceSelected = delegate { };

		public static readonly DependencyProperty GenderProperty = DependencyProperty.Register("Gender", typeof(Gender), typeof(RaceSelection), new PropertyMetadata(new PropertyChangedCallback(RaceSelection.OnGenderPropertyChanged)));
		public Gender Gender
		{
			get { return (Gender)GetValue(GenderProperty); }
			set { SetValue(GenderProperty, value); }
		}

		public Race SelectedRace
		{
			get { return (Race)GetValue(SelectedRaceProperty); }
			set { SetValue(SelectedRaceProperty, value); }
		}
		public static readonly DependencyProperty SelectedRaceProperty = DependencyProperty.Register("SelectedRace", typeof(Race), typeof(RaceSelection), new PropertyMetadata(Game.Races["Norvic"]));			

		public RaceSelection()
		{
			this.Loaded += new RoutedEventHandler(RaceSelection_Loaded);
			InitializeComponent();
		}

		void RaceSelection_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private static void OnGenderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as RaceSelection).SetControlValues();
		}

		private void norvic_Click(object sender, RoutedEventArgs e)
		{
			this.SelectedRace = Game.Races["Norvic"];
			this.SetSelection(norvic);
		}

		private void najii_Click(object sender, RoutedEventArgs e)
		{
			this.SelectedRace = Game.Races["Najii"];
			this.SetSelection(najii);
		}

		private void peren_Click(object sender, RoutedEventArgs e)
		{
			this.SelectedRace = Game.Races["Peren"];
			this.SetSelection(peren);
		}

		private void xhin_Click(object sender, RoutedEventArgs e)
		{
			this.SelectedRace = Game.Races["Xhin"];
			this.SetSelection(xhin);
		}

		public void SetDefaultSelection()
		{
			norvic_Click(norvic, new RoutedEventArgs());
		}

		private void SetSelection(Lionsguard.Icon icon)
		{
			norvic.Selected = najii.Selected = peren.Selected = xhin.Selected = false;
			icon.Selected = true;

			this.SetControlValues();

			this.RaceSelected(this, EventArgs.Empty);
		}

		private void SetControlValues()
		{
			lblName.Text = this.SelectedRace.Name;
			lblDesc.Text = this.SelectedRace.Description;

			norvic.Source = Asset.GetImageSource(String.Format(Asset.AVATAR_FORMAT, "Norvic", this.Gender));
			najii.Source = Asset.GetImageSource(String.Format(Asset.AVATAR_FORMAT, "Najii", this.Gender));
			peren.Source = Asset.GetImageSource(String.Format(Asset.AVATAR_FORMAT, "Peren", this.Gender));
			xhin.Source = Asset.GetImageSource(String.Format(Asset.AVATAR_FORMAT, "Xhin", this.Gender));

			imgRace.Source = Asset.GetImageSource(String.Format(Asset.AVATAR_FORMAT, this.SelectedRace.Name, this.Gender));

			attributes.Strength = this.SelectedRace.Strength;
			attributes.Dexterity = this.SelectedRace.Dexterity;
			attributes.Stamina = this.SelectedRace.Stamina;
			attributes.Beauty = this.SelectedRace.Beauty;

			attributes.Intelligence = this.SelectedRace.Intelligence;
			attributes.Perception = this.SelectedRace.Perception;
			attributes.Endurance = this.SelectedRace.Endurance;
			attributes.Affinity = this.SelectedRace.Affinity;
		}
	}
}
