using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Lionsguard
{
	[TemplatePart(Name = "RootElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "ImageElement", Type = typeof(Image))]
	[TemplatePart(Name = "MouseOverVisualElement", Type = typeof(Border))]
	[TemplatePart(Name = "SelectedVisualElement", Type = typeof(Border))]
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "UnSelected", GroupName = "IconStates")]
	[TemplateVisualState(Name = "Selected", GroupName = "IconStates")]
	public class Icon : Control
	{
		public FrameworkElement RootElement { get; set; }	
		private Image ImageElement { get; set; }
		private Border MouseOverVisualElement { get; set; }
		private Border SelectedVisualElement { get; set; }

		private bool _isMouseOver = false;
		private bool _isPressed = false;

		public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(Icon), new PropertyMetadata(new PropertyChangedCallback(Icon.OnSelectedPropertyChanged)));
		public bool Selected
		{
			get { return (bool)this.GetValue(SelectedProperty); }
			set { this.SetValue(SelectedProperty, value); }
		}

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Icon), new PropertyMetadata(new PropertyChangedCallback(Icon.OnSourcePropertyChanged)));
		public ImageSource Source
		{
			get { return (ImageSource)this.GetValue(SourceProperty); }
			set { this.SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(Icon), null);
		public Brush SelectedBrush
		{
			get { return (Brush)this.GetValue(SelectedBrushProperty); }
			set { this.SetValue(SelectedBrushProperty, value); }
		}

		public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(Icon), null);
		public Brush MouseOverBrush
		{
			get { return (Brush)this.GetValue(MouseOverBrushProperty); }
			set { this.SetValue(MouseOverBrushProperty, value); }
		}

		public event RoutedEventHandler Click = delegate { };

		public Icon()
		{
			this.DefaultStyleKey = typeof(Icon);
		}

		private static void OnSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Icon icon = d as Icon;
			icon.GoToState(true);
		}

		private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Icon icon = d as Icon;
			icon.SetElementValues();
		}

		private void SetElementValues()
		{
			if (this.ImageElement != null)
			{
				this.ImageElement.Source = this.Source;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.RootElement = base.GetTemplateChild("RootElement") as FrameworkElement;
			this.ImageElement = base.GetTemplateChild("ImageElement") as Image;
			this.MouseOverVisualElement = base.GetTemplateChild("MouseOverVisualElement") as Border;
			this.SelectedVisualElement = base.GetTemplateChild("SelectedVisualElement") as Border;

			if (this.RootElement != null)
			{
				this.RootElement.MouseEnter += new MouseEventHandler(OnMouseEnter);
				this.RootElement.MouseLeave += new MouseEventHandler(OnMouseLeave);
				this.RootElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
				this.RootElement.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
			}

			this.SetElementValues();

			this.GoToState(false);
		}

		private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isPressed = false;
			this.GoToState(true);
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isPressed = true;
			this.GoToState(true);
			this.Click(this, new RoutedEventArgs());
		}

		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			_isMouseOver = false;
			this.GoToState(true);
		}

		private void OnMouseEnter(object sender, MouseEventArgs e)
		{
			_isMouseOver = true;
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

			// Icon States
			if (this.Selected)
			{
				VisualStateManager.GoToState(this, "Selected", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "UnSelected", useTransitions);
			}
		}
	}
}
