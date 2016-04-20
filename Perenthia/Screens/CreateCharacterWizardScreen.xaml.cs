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

using Radiance.Markup;

using Perenthia.Controls;
using Perenthia.Dialogs;
using Perenthia.Models;

namespace Perenthia.Screens
{
	public partial class CreateCharacterWizardScreen : UserControl, IScreen
	{
		public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register("Avatar", typeof(Avatar), typeof(CreateCharacterWizardScreen), new PropertyMetadata(new PropertyChangedCallback(CreateCharacterWizardScreen.OnAvatarPropertyChanged)));
		public Avatar Avatar
		{
			get { return (Avatar)GetValue(AvatarProperty); }
			set { SetValue(AvatarProperty, value); }
		}	

		public static readonly DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(CreateCharacterWizardStep), typeof(CreateCharacterWizardScreen), new PropertyMetadata(new PropertyChangedCallback(CreateCharacterWizardScreen.OnStepPropertyChanged)));
		public CreateCharacterWizardStep Step
		{
			get { return (CreateCharacterWizardStep)this.GetValue(StepProperty); }
			set { this.SetValue(StepProperty, value); }
		}

		private LoadingCharactersDialog _diagCreateCharacterContent;
		private bool _nameCheckCauseAdvanceStep = false;
		private bool _supressSuccessNameCheckMessage = false;

		public CreateCharacterWizardScreen()
		{
			ServerManager.Instance.Reset();
			this.Loaded += new RoutedEventHandler(CreateCharacterWizardScreen_Loaded);
			InitializeComponent();
		}

		private static void OnStepPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as CreateCharacterWizardScreen).AdvanceStep();
		}

		private static void OnAvatarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as CreateCharacterWizardScreen).SetAvatar();
		}

		void CreateCharacterWizardScreen_Loaded(object sender, RoutedEventArgs e)
		{
			this.Avatar = new Avatar();
			this.Step = CreateCharacterWizardStep.Gender;

			ctlRace.SetDefaultSelection();

			_diagCreateCharacterContent = new LoadingCharactersDialog();
			diagCreateCharacter.DialogContent.Add(_diagCreateCharacterContent);

			ctlSkills.MaximumSkillValue = 32;
			ctlSkills.SkillChanged += new SkillChangedEventHandler(ctlSkills_SkillChanged);
		}

		private void ctlSkills_SkillChanged(object sender, SkillChangedEventArgs e)
		{
			this.Avatar.Skills[e.Skill.Name].Value = e.Skill.Value;
			this.SetAvatar();
		}

		private void ctlGender_Selected(Perenthia.Controls.GenderSelectedEventArgs e)
		{
			if (this.Avatar != null)
			{
				this.Avatar.Gender = e.Gender;
				this.SetAvatar();
			}
			ctlRace.Gender = e.Gender;
		}

		private void ctlRace_RaceSelected(object sender, EventArgs e)
		{
			if (this.Avatar != null)
			{
				this.Avatar.Race = ctlRace.SelectedRace;
				this.SetAvatar();
			}
			ctlAttributes.SelectedRace = ctlRace.SelectedRace;
		}

		private void ctlName_NameChanged(object sender, TextChangedEventArgs e)
		{
			if (this.Avatar != null)
			{
				this.Avatar.Name = ctlName.AvatarName;
				this.SetAvatar();
			}
		}

		private void ctlName_NameCheckComplete(NameCheckEventArgs e)
		{
			this.Cursor = Cursors.Arrow;
			if (e.IsAvailable)
			{
				if (!_supressSuccessNameCheckMessage)
				{
					MessageBox.Show(e.Message, "Character Name Available", MessageBoxButton.OK);
				}
				if (_nameCheckCauseAdvanceStep)
				{
					_nameCheckCauseAdvanceStep = false;
					this.Step = CreateCharacterWizardStep.Race;
				}
			}
			else
			{
				MessageBox.Show(e.Message, "Unavailable Character Name", MessageBoxButton.OK);
				this.Step = CreateCharacterWizardStep.Name;
			}
		}

		private void ctlAttributes_AttributeChanged(object sender, AttributeChangedEventArgs e)
		{
			switch (e.Name)
			{	
				case "Strength":
					this.Avatar.Strength = e.NewValue;
					break;
				case "Dexterity":
					this.Avatar.Dexterity = e.NewValue;
					break;
				case "Stamina":
					this.Avatar.Stamina = e.NewValue;
					break;
				case "Beauty":
					this.Avatar.Beauty = e.NewValue;
					break;
				case "Intelligence":
					this.Avatar.Intelligence = e.NewValue;
					break;
				case "Perception":
					this.Avatar.Perception = e.NewValue;
					break;
				case "Endurance":
					this.Avatar.Endurance = e.NewValue;
					break;
				case "Affinity":
					this.Avatar.Affinity = e.NewValue;
					break;
			}
			this.SetAvatar();
		}

		private void btnCreate_Click(object sender, RoutedEventArgs e)
		{
			List<object> args = new List<object>();
			args.Add(String.Concat("Name:", this.Avatar.Name));
			args.Add(String.Concat("Gender:", this.Avatar.Gender));
			args.Add(String.Concat("Race:", this.Avatar.Race.Name));
			args.Add(String.Concat("Attr_Strength:", this.Avatar.Strength));
			args.Add(String.Concat("Attr_Dexterity:", this.Avatar.Dexterity));
			args.Add(String.Concat("Attr_Stamina:", this.Avatar.Stamina));
			args.Add(String.Concat("Attr_Beauty:", this.Avatar.Beauty));
			args.Add(String.Concat("Attr_Intelligence:", this.Avatar.Intelligence));
			args.Add(String.Concat("Attr_Perception:", this.Avatar.Perception));
			args.Add(String.Concat("Attr_Endurance:", this.Avatar.Endurance));
			args.Add(String.Concat("Attr_Affinity:", this.Avatar.Affinity));

			foreach (var item in this.Avatar.Skills.Values)
			{
				if (item.Value > 0)
				{
					args.Add(String.Concat("Skill_", item.Name, ":", item.Value));
				}
			}

			_diagCreateCharacterContent.SetStatus("Attempting to create your new Character...");
			diagCreateCharacter.Show();
			ServerManager.Instance.Reset();
			ServerManager.Instance.SendUserCommand("CREATECHARACTER",
				new ServerResponseEventHandler(OnCreateCharacterResponse),
				args.ToArray());
		}

		private void OnCreateCharacterResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessCreateCharacterResponse(e.Tags));
		}

		private void ProcessCreateCharacterResponse(RdlTagCollection tags)
		{
			diagCreateCharacter.Close();
			RdlCommandResponse response = tags.GetTags<RdlCommandResponse>(RdlTagName.RESP.ToString(), "CREATECHARACTER").FirstOrDefault();
			if (response != null)
			{
				if (response.Result)
				{
					// Successful character creation, reload the home screen.
					ScreenManager.SetScreen(new HomeScreen());
				}
				else
				{
					MessageBox.Show(String.Format("Character creation failed for the following reason:\n{0}", response.Message), "Character Creation Failed", MessageBoxButton.OK);
				}
			}
			else
			{
				MessageBox.Show("Character creation failed do to a network error. Please try again.", "Character Creation Failed", MessageBoxButton.OK);
			}
		}

		private void btnFreeForm_Click(object sender, RoutedEventArgs e)
		{
			this.LoadSkills(null);
			this.Step = CreateCharacterWizardStep.Skills;
		}

		private void btnFighter_Click(object sender, RoutedEventArgs e)
		{
			// Set Fighter Skills, advance to skills.
			this.LoadSkills(Game.SkillGroups["Fighter"]);
			this.Step = CreateCharacterWizardStep.Skills;
		}

		private void btnCaster_Click(object sender, RoutedEventArgs e)
		{
			// Set Caster Skills, advance to skills.
			this.LoadSkills(Game.SkillGroups["Caster"]);
			this.Step = CreateCharacterWizardStep.Skills;
		}

		private void btnExplorer_Click(object sender, RoutedEventArgs e)
		{
			// Set Explorer Skills, advance to skills.
			this.LoadSkills(Game.SkillGroups["Explorer"]);
			this.Step = CreateCharacterWizardStep.Skills;
		}

		private void btnThief_Click(object sender, RoutedEventArgs e)
		{
			// Set Thief Skills, advance to skills.
			this.LoadSkills(Game.SkillGroups["Thief"]);
			this.Step = CreateCharacterWizardStep.Skills;
		}

		private void LoadSkills(IEnumerable<Skill> presets)
		{
			this.Avatar.Skills.Clear();
			foreach (var item in Game.Skills)
			{
				this.Avatar.Skills.Add(item.Name, new Skill { Name = item.Name, Description = item.Description });
			}
			if (presets != null)
			{
				foreach (var item in presets)
				{
					this.Avatar.Skills[item.Name].Value = item.Value;
				}
			}
			this.SetSkills();
		}

		private void SetSkills()
		{
			ctlSkills.SkillsSource = this.Avatar.Skills.Values;
			ctlSkills.Refresh();
		}

		#region IScreen Members

		public UIElement Element
		{
			get { return this; }
		}

		public void OnAddedToHost()
		{
		}

		public void OnRemovedFromHost()
		{
		}

		#endregion

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			CreateCharacterWizardStep step = this.Step;
			switch (this.Step)
			{	
				case CreateCharacterWizardStep.Gender:
					ScreenManager.SetScreen(new HomeScreen());
					return;
				case CreateCharacterWizardStep.Name:
					_supressSuccessNameCheckMessage = false;
					step = CreateCharacterWizardStep.Gender;
					btnBack.Visibility = Visibility.Visible;
					break;
				case CreateCharacterWizardStep.Race:
					step = CreateCharacterWizardStep.Name;
					break;
				case CreateCharacterWizardStep.Attributes:
					step = CreateCharacterWizardStep.Race;
					break;
				case CreateCharacterWizardStep.SkillGroups:
					step = CreateCharacterWizardStep.Attributes;
					break;
				case CreateCharacterWizardStep.Skills:
					step = CreateCharacterWizardStep.SkillGroups;
					break;
				case CreateCharacterWizardStep.Review:
					step = CreateCharacterWizardStep.SkillGroups;
					break;
			}
			this.Step = step;
		}

		private void btnContinue_Click(object sender, RoutedEventArgs e)
		{
			// Validate the current step before setting it to the next step.
			string message;
			if (!this.ValidateStep(this.Step, out message))
			{
				MessageBox.Show(message, "Validation Error", MessageBoxButton.OK);
			}
			else
			{
				CreateCharacterWizardStep step = this.Step;
				switch (this.Step)
				{
					case CreateCharacterWizardStep.Gender:
						step = CreateCharacterWizardStep.Name;
						break;
					case CreateCharacterWizardStep.Name:
						if (!ctlName.IsNameAvailable)
						{
							this.Cursor = Cursors.Wait;
							_nameCheckCauseAdvanceStep = true;
							_supressSuccessNameCheckMessage = true;
							ctlName.CheckName();
						}
						else
						{
							step = CreateCharacterWizardStep.Race;
						}
						break;
					case CreateCharacterWizardStep.Race:
						step = CreateCharacterWizardStep.Attributes;
						break;
					case CreateCharacterWizardStep.Attributes:
						step = CreateCharacterWizardStep.SkillGroups;
						break;
					case CreateCharacterWizardStep.SkillGroups:
						step = CreateCharacterWizardStep.Review;
						break;
					case CreateCharacterWizardStep.Skills:
						step = CreateCharacterWizardStep.Review;
						break;
					case CreateCharacterWizardStep.Review:
						// Create Character.
						break;
				}
				this.Step = step;
			}
		}

		private void SetAvatar()
		{
			ctlSummary.SetAvatar(this.Avatar);
		}

		private void AdvanceStep()
		{
			GenderCanvas.Visibility = NameCanvas.Visibility = RaceCanvas.Visibility = SkillGroupsCanvas.Visibility = 
				SkillsCanvas.Visibility = ReviewCanvas.Visibility = AttributesCanvas.Visibility = Visibility.Collapsed;

			switch (this.Step)
			{
				case CreateCharacterWizardStep.Gender:
					lblTitle.Text = "CHARACTER GENDER";
					GenderCanvas.Visibility = Visibility.Visible;
					btnBack.Visibility = Visibility.Collapsed;
					break;
				case CreateCharacterWizardStep.Name:
					lblTitle.Text = "CHARACTER NAME";
					NameCanvas.Visibility = Visibility.Visible;
					btnBack.Visibility = Visibility.Visible;
					break;
				case CreateCharacterWizardStep.Race:
					lblTitle.Text = "CHARACTER RACE";
					RaceCanvas.Visibility = Visibility.Visible;
					break;
				case CreateCharacterWizardStep.Attributes:
					lblTitle.Text = "CHARACTER ATTRIBUTES";
					AttributesCanvas.Visibility = Visibility.Visible;
					break;
				case CreateCharacterWizardStep.SkillGroups:
					lblTitle.Text = "CHARACTER SKILLS";
					SkillGroupsCanvas.Visibility = Visibility.Visible;
					btnContinue.Visibility = Visibility.Visible;
					break;
				case CreateCharacterWizardStep.Skills:
					lblTitle.Text = "CHARACTER SKILLS";
					SkillsCanvas.Visibility = Visibility.Visible;
					btnContinue.Visibility = Visibility.Visible;
					this.SetSkills();
					break;
				case CreateCharacterWizardStep.Review:
					lblTitle.Text = "CHARACTER REVIEW";
					ReviewCanvas.Visibility = Visibility.Visible;
					btnContinue.Visibility = Visibility.Collapsed;
					break;
			}
			bdrTitle.Width = lblTitle.ActualWidth + 8;

			this.SetAvatar();
		}

		private bool ValidateStep(CreateCharacterWizardStep step, out string message)
		{
			// Validate current step.
			switch (step)
			{	
				case CreateCharacterWizardStep.Gender:
					break;
				case CreateCharacterWizardStep.Name:
					if (String.IsNullOrEmpty(this.Avatar.Name))
					{
						message = "Please enter a first name for your new Character.";
						return false;
					}
					break;
				case CreateCharacterWizardStep.Attributes:
					if (ctlAttributes.AttributePoints != 0)
					{
						message = "You still have Attribute Points left over, you need to use these before you can continue with the Character Creation process.";
						return false;
					}
					break;
				case CreateCharacterWizardStep.Race:
					break;
				case CreateCharacterWizardStep.SkillGroups:
					break;
				case CreateCharacterWizardStep.Skills:
					break;
				case CreateCharacterWizardStep.Review:
					break;
			}
			message = String.Empty;
			return true;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			ScreenManager.SetScreen(new HomeScreen());
		}
	}

	public enum CreateCharacterWizardStep
	{
		Gender,
		Name,
		Attributes,
		Race,
		SkillGroups,
		Skills,
		Review,
	}
}
