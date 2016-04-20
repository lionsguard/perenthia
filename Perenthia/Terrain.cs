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

namespace Perenthia
{
	public class Terrain
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int Color { get; set; }
		public string ImageUri { get; set; }	

		public Color GetColor()
		{
			Color color = new Color();
			color.A = (byte)((this.Color & -16777216) >> 0x18);
			color.R = (byte)((this.Color & 0xff0000) >> 0x10);
			color.G = (byte)((this.Color & 0xff00) >> 8);
			color.B = (byte)(this.Color & 0xff);
			return color;
		}
	}
}
