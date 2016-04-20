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

namespace Perenthia.Controls
{
	public partial class AvatarPanel : UserControl
	{
		private bool _isMouseOver = false;
		private Avatar _avatar = null;

		private AvatarDetails _details = null;

		public bool EnableAsButton
		{	
			get { return (bool)GetValue(EnableAsButtonProperty); }
			set { SetValue(EnableAsButtonProperty, value); }
		}
		public static readonly DependencyProperty EnableAsButtonProperty =
			DependencyProperty.Register("EnableAsButton", typeof(bool), typeof(AvatarPanel), new PropertyMetadata(false));

		public bool EnableDetails
		{	
			get { return (bool)GetValue(EnableDetailsProperty); }
			set { SetValue(EnableDetailsProperty, value); }
		}
		public static readonly DependencyProperty EnableDetailsProperty = DependencyProperty.Register("EnableDetails", typeof(bool), typeof(AvatarPanel), new PropertyMetadata(false));

		public bool EnableAffects
		{	
			get { return (bool)GetValue(EnableAffectsProperty); }
			set { SetValue(EnableAffectsProperty, value); }
		}
		public static readonly DependencyProperty EnableAffectsProperty = DependencyProperty.Register("EnableAffects", typeof(bool), typeof(AvatarPanel), new PropertyMetadata(false));
			
		public int AvatarID { get; set; }

		public event RoutedEventHandler Click = delegate { };

		public AvatarPanel()
		{
			this.Loaded += new RoutedEventHandler(AvatarPanel_Loaded);
			InitializeComponent();
		}

		private void AvatarPanel_Loaded(object sender, RoutedEventArgs e)
		{
			//imgAvatar.Clip = new EllipseGeometry() { Center = new Point(32, 32), RadiusX = 32, RadiusY = 32 };

			this.MouseLeftButtonDown += new MouseButtonEventHandler(AvatarPanel_MouseLeftButtonDown);
			this.MouseEnter += new MouseEventHandler(AvatarPanel_MouseEnter);
			this.MouseLeave += new MouseEventHandler(AvatarPanel_MouseLeave);
		}

		private void AvatarPanel_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.EnableAsButton)
			{
				_isMouseOver = false;
				this.GoToState(true);
			}
		}

		private void AvatarPanel_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.EnableAsButton)
			{
				_isMouseOver = true;
				this.GoToState(true);
			}
		}

		private void AvatarPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Click(this, new RoutedEventArgs());
		}

		public Avatar GetAvatar()
		{
			return _avatar;
		}

		private void GoToState(bool useTransitions)
		{
			if (_isMouseOver)
			{
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Normal", useTransitions);
			}
		}

		public void SetProperties(RdlActor actor)
		{
			this.SetProperties(new Avatar(actor));
			//int health, healthMax, willpower, willpowerMax, level;
			//string race, gender;
			//health = actor.Properties.GetValue<int>("Body");
			//healthMax = actor.Properties.GetValue<int>("BodyMax");
			//willpower = actor.Properties.GetValue<int>("Mind");
			//willpowerMax = actor.Properties.GetValue<int>("MindMax");
			//level = actor.Properties.GetValue<int>("Level");
			//race = actor.Properties.GetValue<string>("Race");
			//gender = actor.Properties.GetValue<string>("Gender");

			//this.SetProperties(actor.ID, actor.Name, health, healthMax, willpower, willpowerMax, level, race, gender);
		}

		public void SetProperties(Avatar avatar)
		{
			_avatar = avatar;
			this.SetProperties(avatar.ID, avatar.Name, avatar.Body, avatar.BodyMax, 
				avatar.Mind, avatar.MindMax, avatar.Level, avatar.Race.Name, avatar.Gender.ToString());

			icoAdmin.Visibility = (avatar.Properties.GetValue<bool>("IsAdmin") ? Visibility.Visible : Visibility.Collapsed);
		}

		private void SetProperties(int id, string name, int health, int healthMax, int willpower, int willpowerMax, int level,
			string race, string gender)
		{
			this.AvatarID = id;

			lblLevel.Text = level.ToString();
			lblName.Text = name.ToString();

			statHealth.Maximum = healthMax;
			statHealth.Value = health;
			ToolTipService.SetToolTip(statHealth, String.Format("Health {0}/{1}", health, healthMax));

			statWillpower.Maximum = willpowerMax;
			statWillpower.Value = willpower;
			ToolTipService.SetToolTip(statWillpower, String.Format("Willpower {0}/{1}", willpower, willpowerMax));

			imgAvatar.Source = this.GetImageSource();
            ToolTipService.SetToolTip(imgAvatar, name);

            this.RefreshAffects();
            this.RefreshAwards();
		}

		private ImageSource GetImageSource()
		{
			return _avatar.ImageSource;
		}

		public void RefreshAffects()
		{
			if (this.EnableAffects)
			{
				// Set Affect icons.
				var affects = _avatar.Properties.Values.Where(p => p.Name.StartsWith("Affect_"));
				AffectContainer.Children.Clear();
				foreach (var affect in affects)
				{
					AffectIcon icon = new AffectIcon(_avatar);
					icon.Affect = affect;

					AffectContainer.Children.Add(icon);
				}
			}
		}

        public void RefreshAwards()
        {
            if (_avatar != null)
            {
                var awards = _avatar.Properties.Values.Where(p => p.Name.StartsWith("Award_"));
                // TODO: Where are awards going to go?
            }
        }

		private void lblName_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.EnableDetails && _avatar != null)
			{
				_details = new AvatarDetails();
				_details.Show(_avatar);
				PopupManager.Add(_details, e.GetPosition(null));
			}
		}

		private void lblName_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.EnableDetails && _avatar != null && _details != null)
			{
				_details.Hide();
				PopupManager.Remove();
			}
		}

		private void MenuElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

		}
	}
}
