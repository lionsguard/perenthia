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
using Perenthia.Models;

namespace Perenthia.Dialogs
{
	public partial class ResetCharacterDialog : UserControl
	{
		private enum Step
		{
			Attributes,
			SkillGroups,
			Skills,
			Complete,
		}

		public Avatar Avatar { get; set; }
		private Step CurrentStep { get; set; }

		private LoadingCharactersDialog _loading;

		public event EventHandler Completed = delegate { };

		public ResetCharacterDialog()
		{
			this.Loaded += new RoutedEventHandler(ResetCharacterDialog_Loaded);
			InitializeComponent();
		}

		private void ResetCharacterDialog_Loaded(object sender, RoutedEventArgs e)
		{
			_loading = new LoadingCharactersDialog();
			diagAlert.DialogContent.Add(_loading);

			ctlSkills.MaximumSkillValue = 32;
		}

		private void btnNext_Click(object sender, RoutedEventArgs e)
		{
			if (this.ValidateStep())
			{
				switch (this.CurrentStep)
				{
					case Step.Attributes:
						this.CurrentStep = Step.SkillGroups;
						break;
					case Step.SkillGroups:
						this.CurrentStep = Step.Skills;
						break;
					case Step.Skills:
						this.CurrentStep = Step.Complete;
						break;
					default:
						this.CurrentStep = Step.Attributes;
						break;
				}
				this.RenderStep();
			}
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			switch (this.CurrentStep)
			{
				case Step.SkillGroups:
					this.CurrentStep = Step.Attributes;
					break;
				case Step.Skills:
					this.CurrentStep = Step.SkillGroups;
					break;
				case Step.Complete:
					this.CurrentStep = Step.Skills;
					break;
			}
			this.RenderStep();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ctlAttributes_AttributeChanged(object sender, Perenthia.Controls.AttributeChangedEventArgs e)
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
		}

		private void ctlSkills_SkillChanged(object sender, Perenthia.Controls.SkillChangedEventArgs e)
		{
			this.Avatar.Skills[e.Skill.Name].Value = e.Skill.Value;
		}

		private void btnFighter_Click(object sender, RoutedEventArgs e)
		{
			this.LoadSkills(Game.SkillGroups["Fighter"]);
			this.CurrentStep = Step.Skills;
			this.RenderStep();
		}

		private void btnCaster_Click(object sender, RoutedEventArgs e)
		{
			this.LoadSkills(Game.SkillGroups["Caster"]);
			this.CurrentStep = Step.Skills;
			this.RenderStep();
		}

		private void btnThief_Click(object sender, RoutedEventArgs e)
		{
			this.LoadSkills(Game.SkillGroups["Thief"]);
			this.CurrentStep = Step.Skills;
			this.RenderStep();
		}

		private void btnExplorer_Click(object sender, RoutedEventArgs e)
		{
			this.LoadSkills(Game.SkillGroups["Explorer"]);
			this.CurrentStep = Step.Skills;
			this.RenderStep();
		}

		private void btnFreeForm_Click(object sender, RoutedEventArgs e)
		{
			this.LoadSkills(null);
			this.CurrentStep = Step.Skills;
			this.RenderStep();
		}

		private bool ValidateStep()
		{
			string message = String.Empty;
			switch (this.CurrentStep)
			{	
				case Step.Attributes:
					// Need to have added all available attribute points.
					if (ctlAttributes.AttributePoints != 0)
					{
						message = "You still have Attribute Points left over, you need to use these before you can continue with the Character Reset process.";
					}
					break;
				case Step.Skills:
					// Need to have added all available skill points.
					if (ctlSkills.SkillPoints != 0)
					{
						message = "You still have Skill Points left over, you need to use these before you can continue with the Character Reset process.";
					}
					break;
			}
			if (!String.IsNullOrEmpty(message))
			{
				MessageBox.Show(message, "Character Reset Error", MessageBoxButton.OK);
				return false;
			}
			return true;
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

		public void RenderStep()
		{
			switch (this.CurrentStep)
			{	
				case Step.Attributes:
					ctlAttributes.Visibility = Visibility.Visible;
					SkillGroupsContainer.Visibility = Visibility.Collapsed;
					SkillsContainer.Visibility = Visibility.Collapsed;
					btnNext.Content = "NEXT >>";
					btnNext.Visibility = Visibility.Visible;
					btnCancel.Visibility = Visibility.Visible;
					btnBack.Visibility = Visibility.Collapsed;
					break;
				case Step.SkillGroups:
					ctlAttributes.Visibility = Visibility.Collapsed;
					SkillGroupsContainer.Visibility = Visibility.Visible;
					SkillsContainer.Visibility = Visibility.Collapsed;
					btnNext.Content = "NEXT >>";
					btnBack.Visibility = Visibility.Visible;
					break;
				case Step.Skills:
					ctlAttributes.Visibility = Visibility.Collapsed;
					SkillGroupsContainer.Visibility = Visibility.Collapsed;
					SkillsContainer.Visibility = Visibility.Visible;
					btnNext.Content = "Finish";
					btnBack.Visibility = Visibility.Visible;
					break;
				case Step.Complete:
					ctlAttributes.Visibility = Visibility.Collapsed;
					SkillGroupsContainer.Visibility = Visibility.Collapsed;
					SkillsContainer.Visibility = Visibility.Collapsed;
					btnCancel.Visibility = Visibility.Collapsed;
					btnNext.Visibility = Visibility.Collapsed;
					btnBack.Visibility = Visibility.Collapsed;
					this.SendCompletedData();
					break;
			}
		}

		private void SendCompletedData()
		{
			// TODO: Display loading window.

			List<object> args = new List<object>();
			args.Add(String.Concat("ID:", this.Avatar.ID));
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

			_loading.SetStatus("Attempting to reset your Character...");
			diagAlert.Show();
			ServerManager.Instance.Reset();
			ServerManager.Instance.SendUserCommand("RESETCHARACTER",
				new ServerResponseEventHandler(OnResetCharacterResponse),
				args.ToArray());
		}

		private void OnResetCharacterResponse(ServerResponseEventArgs e)
		{
			this.Dispatcher.BeginInvoke(() => this.ProcessResetCharacterResponse(e.Tags));
		}

		private void ProcessResetCharacterResponse(RdlTagCollection tags)
		{
			diagAlert.Close();
			RdlCommandResponse response = tags.GetTags<RdlCommandResponse>(RdlTagName.RESP.ToString(), "RESETCHARACTER").FirstOrDefault();
			if (response != null)
			{
				if (response.Result)
				{
					// Successful character reset, reload the home screen.
					this.Completed(this, EventArgs.Empty);
					ScreenManager.SetScreen(new Perenthia.Screens.HomeScreen());
				}
				else
				{
					MessageBox.Show(String.Format("Character reset failed for the following reason:\n{0}", response.Message), "Character Reset Failed", MessageBoxButton.OK);
				}
			}
			else
			{
				MessageBox.Show("Character reset failed do to a network error. Please try again.", "Character Reset Failed", MessageBoxButton.OK);
			}
			this.Close();
		}


		private void OnSavePlayerCommandResponse()
		{
			this.Completed(this, EventArgs.Empty);
			this.Close();
		}

		public void Show(Avatar avatar)
		{
			this.Avatar = avatar;
			this.Visibility = Visibility.Visible;

			ctlAttributes.SelectedRace = avatar.Race;

			// Name
			lblName.Text = avatar.Name;

			this.CurrentStep = Step.Attributes;
			this.RenderStep();

			MessageBox.Show("Your Character requires a reset, you will need to re-select your Attributes and Skills in order to continue playing with this Character.", "Character Reset", MessageBoxButton.OK);
		}

		public void Close()
		{
			this.Visibility = Visibility.Collapsed;
		}
	}
}
