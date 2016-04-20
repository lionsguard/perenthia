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

namespace Perenthia.Controls
{
	public partial class SkillPanel : UserControl
	{
		public static readonly DependencyProperty SkillProperty = DependencyProperty.Register("Skill", typeof(Skill), typeof(SkillPanel), new PropertyMetadata(new PropertyChangedCallback(SkillPanel.OnSkillPropertyChanged)));
		public Skill Skill
		{
			get { return (Skill)this.GetValue(SkillProperty); }
			set { this.SetValue(SkillProperty, value); }
		}

		public static readonly DependencyProperty EnableButtonsProperty = DependencyProperty.Register("EnableButtons", typeof(bool), typeof(SkillPanel), new PropertyMetadata(false, new PropertyChangedCallback(SkillPanel.OnEnableButtonsPropertyChanged)));
		public bool EnableButtons
		{
			get { return (bool)this.GetValue(EnableButtonsProperty); }
			set { this.SetValue(EnableButtonsProperty, value); }
		}

		public int Maximum
		{
			get { return (int)pgMeter.Maximum; }
			set { pgMeter.Maximum = value; }
		}

		public event SkillChangedEventHandler SkillChanging = delegate { };
		public event SkillChangedEventHandler SkillChanged = delegate { };

		public SkillPanel()
		{
			InitializeComponent();
		}

		public SkillPanel(Skill skill)
			: this(skill, true)
		{
		}

		public SkillPanel(Skill skill, bool enabledButtons)
			: this()
		{
			this.EnableButtons = enabledButtons;
			this.Skill = skill;
			this.BindSkill();
			this.SetButtonVisibility();
		}

		private static void OnSkillPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as SkillPanel).BindSkill();
		}

		private static void OnEnableButtonsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as SkillPanel).SetButtonVisibility();
		}

		private void BindSkill()
		{
			lblName.Text = this.Skill.Name.Replace("Skill_", String.Empty);
			lblDesc.Text = this.Skill.Description;
			pgMeter.Value = this.Skill.Value;
			lblValue.Text = this.Skill.Value.ToString();
		}

		private void SetButtonVisibility()
		{
			btnMinus.Visibility = btnPlus.Visibility = (this.EnableButtons ? Visibility.Visible : Visibility.Collapsed);
		}

		private void btnPlus_Click(object sender, RoutedEventArgs e)
		{
			this.RaiseSkillChangedEvent(this.Skill.Value + 1);
		}

		private void btnMinus_Click(object sender, RoutedEventArgs e)
		{
			this.RaiseSkillChangedEvent(this.Skill.Value - 1);
		}

		private void RaiseSkillChangedEvent(int newValue)
		{
			if (newValue > this.Maximum)
			{
				MessageBox.Show("This skill can not be increased any further.", "Skill Increase Error", MessageBoxButton.OK);
				return;
			}
			else if (newValue < 0)
			{
				MessageBox.Show("The skill is already at zero.", "Skill Decrease Error", MessageBoxButton.OK);
				return;
			}

			SkillChangedEventArgs args = new SkillChangedEventArgs(this.Skill, this.Skill.Value, newValue);
			this.SkillChanging(this, args);
			if (!args.Cancel)
			{
				this.Skill.Value = newValue;
				this.BindSkill();
				this.SkillChanged(this, args);
			}
		}
	}

	public delegate void SkillChangedEventHandler(object sender, SkillChangedEventArgs e);
	public class SkillChangedEventArgs : EventArgs
	{
		public Skill Skill { get; set; }
		public int OldValue { get; set; }
		public int NewValue { get; set; }
		public bool Cancel { get; set; }	

		public SkillChangedEventArgs(Skill skill, int oldValue, int newValue)
		{
			this.Skill = skill;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
	}
}
