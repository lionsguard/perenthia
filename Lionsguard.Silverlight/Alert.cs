using System;
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
	[TemplatePart(Name = "MainBorder", Type = typeof(Border))]
	[TemplatePart(Name = "MainImage", Type = typeof(Image))]
	[TemplatePart(Name = "Content", Type = typeof(ContentPresenter))]
	[TemplateVisualState(Name = "None", GroupName = "AlertStates")]
	[TemplateVisualState(Name = "Positive", GroupName = "AlertStates")]
	[TemplateVisualState(Name = "Negative", GroupName = "AlertStates")]
	[System.Windows.Markup.ContentProperty("Content")]
	public class Alert : Control
	{
		private Border ElementMainBorder { get; set; }
		private Image ElementMainImage { get; set; }
		private ContentPresenter ElementContent { get; set; }

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Alert), new PropertyMetadata(new PropertyChangedCallback(Alert.OnPropertyChanged)));
		public ImageSource Source
		{
			get { return (ImageSource)this.GetValue(SourceProperty); }
			set { this.SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(Alert), new PropertyMetadata(new PropertyChangedCallback(Alert.OnPropertyChanged)));
		public object Content
		{
			get { return this.GetValue(ContentProperty); }
			set { this.SetValue(ContentProperty, value); }
		}

		public static readonly DependencyProperty AlertTypeProperty = DependencyProperty.Register("AlertType", typeof(AlertType), typeof(Alert), new PropertyMetadata(new PropertyChangedCallback(Alert.OnAlertTypePropertyChanged)));
		public AlertType AlertType
		{
			get { return (AlertType)this.GetValue(AlertTypeProperty); }
			set { this.SetValue(AlertTypeProperty, value); }
		}

		public Alert()
		{
			this.DefaultStyleKey = typeof(Alert);
		}

		private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Alert).BindProperties();
		}

		private static void OnAlertTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Alert).GoToState(true);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.ElementMainBorder = base.GetTemplateChild("MainBorder") as Border;
			this.ElementMainImage = base.GetTemplateChild("MainImage") as Image;
			this.ElementContent = base.GetTemplateChild("Content") as ContentPresenter;

			this.BindProperties();
			this.GoToState(false);
		}

		private void BindProperties()
		{
			if (this.ElementMainImage != null)
				this.ElementMainImage.Source = this.Source;

			if (this.ElementContent != null)
				this.ElementContent.Content = this.Content;

			
		}

		private void GoToState(bool useTransitions)
		{
			// Alert States
			switch (this.AlertType)
			{
				case AlertType.Positive:
					VisualStateManager.GoToState(this, "Positive", useTransitions);
					break;
				case AlertType.Negative:
					VisualStateManager.GoToState(this, "Negative", useTransitions);
					break;
				default:
					VisualStateManager.GoToState(this, "None", useTransitions);
					break;
			}
		}
	}

	public enum AlertType
	{
		None,
		Positive,
		Negative,
	}
}
