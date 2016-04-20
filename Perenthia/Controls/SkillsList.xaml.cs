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
	public partial class SkillsList : UserControl
	{
		public static readonly int SkillPointsMaximum = 32;

		public static readonly DependencyProperty SkillsSourceProperty = DependencyProperty.Register("SkillsSource", typeof(IEnumerable<Skill>), typeof(SkillsList), new PropertyMetadata(new PropertyChangedCallback(SkillsList.OnSkillsSourcePropertyChanged)));
		public IEnumerable<Skill> SkillsSource
		{
			get { return (IEnumerable<Skill>)this.GetValue(SkillsSourceProperty); }
			set { this.SetValue(SkillsSourceProperty, value); }
		}

		public static readonly DependencyProperty EnableButtonsProperty = DependencyProperty.Register("EnableButtons", typeof(bool), typeof(SkillsList), new PropertyMetadata(true, new PropertyChangedCallback(SkillsList.OnEnableButtonsPropertyChanged)));
		public bool EnableButtons
		{
			get { return (bool)this.GetValue(EnableButtonsProperty); }
			set { this.SetValue(EnableButtonsProperty, value); }
		}

		public static readonly DependencyProperty ShowSkillPointsProperty = DependencyProperty.Register("ShowSkillPoints", typeof(bool), typeof(SkillsList), new PropertyMetadata(true, new PropertyChangedCallback(SkillsList.OnShowSkillPointsPropertyChanged)));
		public bool ShowSkillPoints
		{	
			get { return (bool)GetValue(ShowSkillPointsProperty); }
			set { SetValue(ShowSkillPointsProperty, value); }
		}
				
		public static readonly DependencyProperty SkillPointsProperty = DependencyProperty.Register("SkillPoints", typeof(int), typeof(SkillsList), new PropertyMetadata(SkillPointsMaximum));
		public int SkillPoints
		{
			get { return (int)GetValue(SkillPointsProperty); }
			set { SetValue(SkillPointsProperty, value); }
		}

		public static readonly DependencyProperty MaximumSkillValueProperty = DependencyProperty.Register("MaximumSkillValue", typeof(int), typeof(SkillsList), new PropertyMetadata(new PropertyChangedCallback(SkillsList.OnMaximumSkillValuePropertyChanged)));
		public int MaximumSkillValue
		{
			get { return (int)GetValue(MaximumSkillValueProperty); }
			set { SetValue(MaximumSkillValueProperty, value); }
		}

		public bool ShowUntrainedSkills
		{
			get { return (bool)GetValue(ShowUntrainedSkillsProperty); }
			set { SetValue(ShowUntrainedSkillsProperty, value); }
		}
		public static readonly DependencyProperty ShowUntrainedSkillsProperty = DependencyProperty.Register("ShowUntrainedSkills", typeof(bool), typeof(SkillsList), new PropertyMetadata(true, new PropertyChangedCallback(SkillsList.OnShowUntrainedSkillsPropertyChanged)));
		private static void OnShowUntrainedSkillsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as SkillsList).BindSkills();
		}

		public event SkillChangedEventHandler SkillChanged = delegate { };

		public SkillsList()
		{
			this.Loaded += new RoutedEventHandler(SkillsList_Loaded);
			InitializeComponent();
		}

		void SkillsList_Loaded(object sender, RoutedEventArgs e)
		{
			lblSkillPoints.Text = this.SkillPoints.ToString();
		}

		private static void OnSkillsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as SkillsList).BindSkills();
		}

		private static void OnEnableButtonsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			SkillsList skills = obj as SkillsList;
			foreach (var item in skills.list.Children)
			{
				if (item is SkillPanel)
				{
					(item as SkillPanel).EnableButtons = skills.EnableButtons;
				}
			}
		}
		private static void OnShowSkillPointsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			SkillsList skills = obj as SkillsList;
			Visibility vis = (skills.ShowSkillPoints) ? Visibility.Visible : Visibility.Collapsed;
			skills.lblSkillPoints.Visibility = vis;
			skills.SkillPointLabel.Visibility = vis;
		}
		

		private static void OnMaximumSkillValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as SkillsList).ResetMax();
		}

		public void Refresh()
		{
			this.BindSkills();
		}

		private void BindSkills()
		{
			this.SkillPoints = SkillPointsMaximum;
			list.Children.Clear();
			if (this.SkillsSource != null && this.SkillsSource.Count() > 0)
			{
				// Sort the skills.
				string group = String.Empty;
				Game.EnsureSkillDetails(this.SkillsSource);
				foreach (var item in this.SkillsSource.OrderBy(s => s.GroupName))
				{
					if (group != item.GroupName)
					{	
						group = item.GroupName;
						TextBlock txt = new TextBlock();
						txt.FontFamily = new FontFamily("Georgia");
						txt.FontSize = 12;
						txt.FontWeight = FontWeights.Bold;
						txt.Text = item.GroupName;
						txt.Foreground = Brushes.HeadingBrush;
						txt.Padding = new Thickness(0, 0, 0, 4);
						list.Children.Add(txt);
					}

					if (item.Value <= 0 && !this.ShowUntrainedSkills)
						continue;

					SkillPanel pnl = new SkillPanel(item, this.EnableButtons);
					pnl.Maximum = this.MaximumSkillValue;
					pnl.SkillChanging += new SkillChangedEventHandler(OnSkillChanging);
					pnl.SkillChanged += new SkillChangedEventHandler(OnSkillChanged);
					list.Children.Add(pnl);
					this.SkillPoints -= item.Value;
				}
			}
			if (this.SkillPoints > SkillPointsMaximum) this.SkillPoints = SkillPointsMaximum;
			if (this.SkillPoints < 0) this.SkillPoints = 0;
			lblSkillPoints.Text = this.SkillPoints.ToString();
		}

		private void OnSkillChanging(object sender, SkillChangedEventArgs e)
		{
			if (e.NewValue < e.OldValue)
			{
				this.SkillPoints++;
				if (this.SkillPoints > SkillPointsMaximum)
				{
					this.SkillPoints = SkillPointsMaximum;
					MessageBox.Show("You can not decrease this skill.");
					e.Cancel = true;
				}
			}
			else
			{
				this.SkillPoints--;
				if (this.SkillPoints < 0)
				{
					this.SkillPoints = 0;
					MessageBox.Show("You do not have any Skill Points left.");
					e.Cancel = true;
				}
			}

			lblSkillPoints.Text = this.SkillPoints.ToString();
		}

		private void OnSkillChanged(object sender, SkillChangedEventArgs e)
		{
			this.SkillChanged(sender, e);
		}

		private void ResetMax()
		{
			foreach (var item in list.Children)
			{
				SkillPanel pnl = item as SkillPanel;
				if (pnl != null)
				{
					pnl.Maximum = this.MaximumSkillValue;
				}
			}
		}
	}
}
