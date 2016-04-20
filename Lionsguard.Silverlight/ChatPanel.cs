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
	[TemplatePart(Name = "InputTextBox", Type = typeof(TextBox))]
	[TemplatePart(Name = "SendButton", Type = typeof(Button))]
	[TemplatePart(Name = "MessageViewer", Type = typeof(ScrollViewer))]
	public class ChatPanel : Control
	{
		private StackPanel ElementContent { get; set; }
		private TextBox ElementInputTextBox { get; set; }
		private Button ElementSendButton { get; set; }
		private ScrollViewer ElementMessageViewer { get; set; }

		public int MaxDisplayedItems { get; set; }

		public bool IsKeyDown { get; set; }	
		
		public string InputText
		{
			get
			{
				if (this.ElementInputTextBox != null) return this.ElementInputTextBox.Text;
				return String.Empty;
			}
			set
			{
				if (this.ElementInputTextBox != null) this.ElementInputTextBox.Text = value;
			}
		}

		public object SendButtonContent
		{
			get
			{
				if (this.ElementSendButton != null) return this.ElementSendButton.Content;
				return null;
			}
			set
			{
				if (this.ElementSendButton != null) this.ElementSendButton.Content = value;
			}
		}

		public bool HideCommandBar
		{	
			get { return (bool)GetValue(HideCommandBarProperty); }
			set { SetValue(HideCommandBarProperty, value); }
		}
		public static readonly DependencyProperty HideCommandBarProperty = DependencyProperty.Register("HideCommandBar", typeof(bool), typeof(ChatPanel), new PropertyMetadata(false, new PropertyChangedCallback(ChatPanel.OnHideCommandBarPropertyChanged)));
		private static void OnHideCommandBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ChatPanel).SetCommandBarDisplay();
		}

		public Brush DefaultMessageBrush { get; set; }
		public FontWeight DefaultFontWeight { get; set; }
		public double DefaultFontSize { get; set; }	

		public ChatPanel()
		{
			this.MaxDisplayedItems = 100;
			this.DefaultStyleKey = typeof(ChatPanel);
			this.DefaultFontSize = 10;
			this.DefaultFontWeight = FontWeights.Normal;
			this.DefaultMessageBrush = new SolidColorBrush(Colors.White);
			this.Loaded += new RoutedEventHandler(ChatPanel_Loaded);
		}

		void ChatPanel_Loaded(object sender, RoutedEventArgs e)
		{
			this.ApplyTemplate();
		}

		#region Events
		public event InputReceivedEventHandler InputReceived = delegate { };
		protected virtual void OnInputReceived(InputReceivedEventArgs e)
		{
			this.InputReceived(this, e);
		}
		#endregion

		public void SetCommandBarDisplay()
		{
			Visibility vis = (this.HideCommandBar) ? Visibility.Collapsed : Visibility.Visible;
			double margin = (vis == Visibility.Visible) ? 24 : 0;
			if (this.ElementInputTextBox != null) this.ElementInputTextBox.Visibility = vis;
			if (this.ElementSendButton != null) this.ElementSendButton.Visibility = vis;
			if (this.ElementMessageViewer != null)
			{
				this.ElementMessageViewer.Margin = new Thickness(0, 0, 0, margin);
			}
		}

		public void Clear()
		{
			if (this.ElementContent != null)
			{
				this.ElementContent.Children.Clear();
			}
		}

		public void SetFocus(string text)
		{
			if (this.ElementInputTextBox != null)
			{
				this.ElementInputTextBox.Focus();
				if (!String.IsNullOrEmpty(text))
				{
					this.ElementInputTextBox.Text = text;
				}
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.ElementContent = new StackPanel();
			this.ElementContent.Orientation = Orientation.Vertical;

			this.ElementInputTextBox = base.GetTemplateChild("InputTextBox") as TextBox;
			if (this.ElementInputTextBox == null) this.ElementInputTextBox = new TextBox();
			this.ElementInputTextBox.KeyDown += new KeyEventHandler(ElementInputTextBoxKeyDown);

			this.ElementSendButton = base.GetTemplateChild("SendButton") as Button;
			if (this.ElementSendButton == null) this.ElementSendButton = new Button();
			this.ElementSendButton.Click += new RoutedEventHandler(ElementSendButtonClick);

			this.ElementMessageViewer = base.GetTemplateChild("MessageViewer") as ScrollViewer;
			if (this.ElementMessageViewer == null) this.ElementMessageViewer = new ScrollViewer();
			// Set the content of the scrollviewer to the stack panel.
			this.ElementMessageViewer.Content = this.ElementContent;

			this.SetCommandBarDisplay();
		}

		private void ElementInputTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.ElementSendButtonClick(this.ElementSendButton, new RoutedEventArgs());
			}
			this.IsKeyDown = true;
			//e.Handled = true;
		}

		private void ElementSendButtonClick(object sender, RoutedEventArgs e)
		{
			this.OnInputReceived(new InputReceivedEventArgs(this.ElementInputTextBox.Text));
			this.ElementInputTextBox.Text = String.Empty;
		}

		public void Append(string text)
		{
			this.Append(text, this.DefaultMessageBrush, this.DefaultFontWeight, this.DefaultFontSize);
		}

		private delegate void AppendTextDelegate(string text, Brush brush, FontWeight fontWeight, double fontSize);
		public void Append(string text, Brush brush, FontWeight fontWeight, double fontSize)
		{
			if (this.CheckAccess())
			{
				this.Append(this.CreateText(text, brush, fontWeight, fontSize));
			}
			else
			{
				this.Dispatcher.BeginInvoke(new AppendTextDelegate(this.Append), text, brush, fontWeight, fontSize);
			}
		}

		public void Append(IList<UIElement> list)
		{
			foreach (var item in list)
			{
				this.Append(item);	
			}
		}

		private delegate void AppendDelegate(UIElement element);
		public void Append(UIElement element)
		{
			if (this.CheckAccess())
			{
				if (this.ElementContent.Children.Count >= this.MaxDisplayedItems)
				{
					this.ElementContent.Children.RemoveAt(0);
				}
				this.ElementContent.Children.Add(element);

				double scrollOffset = this.ElementMessageViewer.VerticalOffset;
				scrollOffset += this.ElementMessageViewer.ScrollableHeight + this.ElementMessageViewer.ViewportHeight;
				this.ElementMessageViewer.ScrollToVerticalOffset(scrollOffset);
			}
			else
			{
				this.Dispatcher.BeginInvoke(new AppendDelegate(this.Append), element);
			}
		}

		private delegate TextBlock CreateTextDelegate(string text, Brush brush, FontWeight fontWeight, double fontSize);
		private TextBlock CreateText(string text, Brush brush, FontWeight fontWeight, double fontSize)
		{
			TextBlock txt = new TextBlock();
			txt.Text = text;
			txt.Foreground = brush;
			txt.FontWeight = fontWeight;
			txt.FontSize = fontSize;
			txt.TextWrapping = TextWrapping.Wrap;
			return txt;
		}

		public HyperlinkButton CreateLink(string text, Brush brush, FontWeight fontWeight, double fontSize, object tag, RoutedEventHandler linkCallback)
		{
			TextBlock txt = new TextBlock();
			txt.TextWrapping = TextWrapping.Wrap;
			txt.Text = text;

			HyperlinkButton lnk = new HyperlinkButton();
			lnk.Content = txt;
			lnk.Foreground = brush;
			lnk.FontWeight = fontWeight;
			lnk.FontSize = fontSize;
			lnk.Tag = tag;
			lnk.Click += new RoutedEventHandler(linkCallback);
			return lnk;
		}
	}
}
