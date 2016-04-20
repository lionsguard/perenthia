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

namespace Perenthia.Controls
{
	public partial class GenderSelection : UserControl
	{
		public event GenderSelectedEventHandler Selected = delegate { };

		public static readonly DependencyProperty GenderProperty = DependencyProperty.Register("Gender", typeof(Gender), typeof(GenderSelection), new PropertyMetadata(Gender.Male , new PropertyChangedCallback(GenderSelection.OnGenderPropertyChanged)));
		public Gender Gender
		{
			get { return (Gender)this.GetValue(GenderProperty); }
			set { this.SetValue(GenderProperty, value); }
		}

		public GenderSelection()
		{
			this.Loaded += new RoutedEventHandler(GenderSelection_Loaded);
			InitializeComponent();
		}

		void GenderSelection_Loaded(object sender, RoutedEventArgs e)
		{
			this.SetSelection();
		}

		private static void OnGenderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as GenderSelection).SetSelection();
		}

		private void male_Click(object sender, RoutedEventArgs e)
		{
			this.Gender = Gender.Male;
		}

		private void female_Click(object sender, RoutedEventArgs e)
		{
			this.Gender = Gender.Female;
		}

		private void SetSelection()
		{
			female.Selected = male.Selected = false;
			switch (this.Gender)
			{
				case Gender.Male:
					male.Selected = true;
					break;
				case Gender.Female:
					female.Selected = true;
					break;
			}
			this.Selected(new GenderSelectedEventArgs(this.Gender));
		}
	}

	public delegate void GenderSelectedEventHandler(GenderSelectedEventArgs e);

	public class GenderSelectedEventArgs : EventArgs
	{
		public Gender Gender { get; set; }

		public GenderSelectedEventArgs(Gender gender)
		{
			this.Gender = gender;
		}
	}
}
