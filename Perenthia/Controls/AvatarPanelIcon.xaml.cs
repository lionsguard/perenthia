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
	public partial class AvatarPanelIcon : UserControl
	{
		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(AvatarPanelIcon), new PropertyMetadata(null, new PropertyChangedCallback(AvatarPanelIcon.OnSourcePropertyChanged)));
		private static void OnSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as AvatarPanelIcon).Refresh();
		}

		public string ToolTip
		{
			get { return (string)GetValue(ToolTipProperty); }
			set { SetValue(ToolTipProperty, value); }
		}
		public static readonly DependencyProperty ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(string), typeof(AvatarPanelIcon), new PropertyMetadata(null, new PropertyChangedCallback(AvatarPanelIcon.OnToolTipPropertyChanged)));
		private static void OnToolTipPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as AvatarPanelIcon).SetToolTip();
		}
            
            
		public AvatarPanelIcon()
		{
			InitializeComponent();
		}

		private void Refresh()
		{
			if (this.Source != null)
			{
				imgMain.Source = this.Source;
			}
		}

		private void SetToolTip()
		{
			if (!String.IsNullOrEmpty(this.ToolTip))
			{
				ToolTipService.SetToolTip(this, this.ToolTip);
			}
		}
	}
}
