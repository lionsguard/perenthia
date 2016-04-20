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

using Radiance;
using Radiance.Markup;
using Perenthia.Controls;

namespace Perenthia.Dialogs
{
	public partial class MapDialog : UserControl
	{
		public MapDialog()
		{
			InitializeComponent();
		}

		public void SetViewPort(Geometry clip)
		{
			ctlMap.ViewPortClip = clip;
		}

		public void LoadMap(RdlTagCollection tags)
		{
			ctlMap.LoadMap(tags);
		}

		public void SetView(Point3 center)
		{
			ctlMap.SetView(center);
		}
	}
}
