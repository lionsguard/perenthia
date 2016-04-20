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
	public partial class Tab : UserControl
	{
		public string Title
		{
			get { return TitleLabel.Text; }
			set { TitleLabel.Text = value; }
		}

		public bool IsActive { get; private set; }
			
		public Tab()
		{
			InitializeComponent();
		}

		public void DeactivateTab()
		{
			this.IsActive = false;
			VisualStateManager.GoToState(this, "Normal", true);
			BlinkTabAnimation.Stop();
		}

		public void ActivateTab()
		{
			this.IsActive = true;
			VisualStateManager.GoToState(this, "Active", true);
			BlinkTabAnimation.Stop();
		}

		public void BlinkTab()
		{
			if (!this.IsActive)
			{
				BlinkTabAnimation.Begin();
			}
		}
	}
}
