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
	public partial class SkillNameValue : UserControl
	{
		public static readonly DependencyProperty SkillProperty = DependencyProperty.Register("Skill", typeof(Skill), typeof(SkillNameValue), new PropertyMetadata(new PropertyChangedCallback(SkillNameValue.OnSkillPropertyChanged)));
		public Skill Skill
		{
			get { return (Skill)this.GetValue(SkillProperty); }
			set { this.SetValue(SkillProperty, value); }
		}

		public SkillNameValue()
		{
			this.Loaded += new RoutedEventHandler(SkillNameValue_Loaded);
			InitializeComponent();
		}

		public SkillNameValue(Skill skill)
			: this()
		{
			this.Skill = skill;
		}

		private static void OnSkillPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as SkillNameValue).BindSkill();
		}

		private void SkillNameValue_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void BindSkill()
		{
			lblName.Text = this.Skill.Name;
			lblValue.Text = this.Skill.Value.ToString();
		}
	}
}
