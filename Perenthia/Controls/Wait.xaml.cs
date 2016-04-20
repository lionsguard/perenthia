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
	public partial class Wait : UserControl
	{
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Wait), new PropertyMetadata(null, new PropertyChangedCallback(Wait.OnTextPropertyChanged)));
		private static void OnTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Wait).Refresh();
		}

		public Wait()
		{
			this.Loaded += new RoutedEventHandler(Wait_Loaded);
			InitializeComponent();
		}

		private void Wait_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void Refresh()
		{
			txtContent.Text = this.Text;
		}

		public void Show(string text)
		{
			this.Text = text;
			this.Show();
		}

		public void Show()
		{
            pgMain.IsEnabled = true;
			this.Visibility = Visibility.Visible;
		}

		public void Hide()
        {
            pgMain.IsEnabled = false;
			this.Visibility = Visibility.Collapsed;
		}
	}
}
