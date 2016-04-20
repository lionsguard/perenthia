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

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	public class MapDetail
	{
		public string Name { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int StartX { get; set; }
		public int StartY { get; set; }
		public int EndX { get; set; }
		public int EndY { get; set; }

		public double CenterX
		{
			get { return this.StartX + ((this.EndX - this.StartX) * 0.5); }
		}

		public double CenterY
		{
			get { return this.StartY + ((this.EndY - this.StartY) * 0.5); }
		}

		public MapDetail()
		{
		}

		public MapDetail(RdlTag tag)
		{
			this.Name = tag.GetArg<string>(0);
			this.Width = tag.GetArg<int>(1);
			this.Height = tag.GetArg<int>(2);
			this.StartX = tag.GetArg<int>(3);
			this.StartY = tag.GetArg<int>(4);
			this.EndX = tag.GetArg<int>(5);
			this.EndY = tag.GetArg<int>(6);
		}

		public override string ToString()
		{
			return String.Format("{0} ({1}, {2})", this.Name, this.Width, this.Height);
		}
	}
	public class MapKey
	{
		public int StartX { get; set; }
		public int StartY { get; set; }
		public int EndX { get; set; }
		public int EndY { get; set; }

		public int CenterX
		{
			get { return this.StartX + (int)((this.EndX - this.StartX) * 0.5); }
		}

		public int CenterY
		{
			get { return this.StartY + (int)((this.EndY - this.StartY) * 0.5); }
		}

		public MapKey(int startX, int startY, int endX, int endY)
		{
			this.StartX = startX;
			this.StartY = startY;
			this.EndX = endX;
			this.EndY = endY;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is MapKey)
			{
				MapKey key = (MapKey)obj;
				if (key.StartX >= this.StartX && key.EndX <= this.EndX)
				{
					return (key.StartY >= this.StartY && key.EndY <= this.EndY);
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.StartX.GetHashCode() + this.EndX.GetHashCode() + this.StartY.GetHashCode() + this.EndY.GetHashCode();
		}
	}
}
