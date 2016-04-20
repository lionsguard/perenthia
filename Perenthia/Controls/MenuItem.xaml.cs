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
	public partial class MenuItem : UserControl
	{
		private bool _isMouseOver = false;
		private bool _isPressed = false;

		public event EventHandler Click = delegate { };

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MenuItem), new PropertyMetadata(null, new PropertyChangedCallback(MenuItem.OnTitlePropertyChanged)));
		private static void OnTitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			MenuItem item = obj as MenuItem;
			if (item != null)
			{
				item.lblName.Text = item.Title;
			}
		}

		public MenuItem()
		{
			InitializeComponent();
		}

		private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isPressed = true;
			this.GoToState(true);

			this.Click(this, e);
		}

		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isPressed = false;
			this.GoToState(true);
		}

		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			_isMouseOver = true;
			this.GoToState(true);
		}

		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			_isMouseOver = false;
			this.GoToState(true);
		}

		private void GoToState(bool useTransitions)
		{
			// Common States
			if (_isPressed)
			{
				VisualStateManager.GoToState(this, "Pressed", useTransitions);
			}
			else if (_isMouseOver)
			{
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Normal", useTransitions);
			}
		}
	}
}
