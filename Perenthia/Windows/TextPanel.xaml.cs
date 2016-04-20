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
using Lionsguard;

namespace Perenthia.Windows
{
	public partial class TextPanel : UserControl, ITextWindow
	{
		private FrameworkElement _lastElement = null;

		public TextPanel()
		{
			InitializeComponent();
		}

		#region ITextWindow Members

		public StackPanel TextContainer
		{
			get { return TextList; }
		}

		public void Append(TextType type, string text, object tag, RoutedEventHandler linkCallback)
		{
			_lastElement = this.AppendTextBlock(type, text, tag, linkCallback);
			this.ScrollToEnd();
		}

		public void ScrollToEnd()
		{
			Logger.LogDebug("TextPanel.ScrollToEnd: this.TextScrollViewer.VerticalOffset = {0}", this.TextScrollViewer.VerticalOffset);
			Logger.LogDebug("TextPanel.ScrollToEnd: this.TextScrollViewer.ScrollableHeight = {0}", this.TextScrollViewer.ScrollableHeight);
			//Logger.LogDebug("TextPanel.ScrollToEnd: this.TextScrollViewer.ViewportHeight = {0}", this.TextScrollViewer.ViewportHeight);
			//double scrollOffset = this.TextScrollViewer.VerticalOffset;
			//scrollOffset += this.TextScrollViewer.ScrollableHeight + this.TextScrollViewer.ViewportHeight + 100;
			//Logger.LogDebug("TextPanel.ScrollToEnd: scrollOffset = {0}", scrollOffset);
			//this.TextScrollViewer.ScrollToVerticalOffset(scrollOffset);

			this.TextScrollViewer.ScrollToBottom();
		}

		#endregion
	}
}
