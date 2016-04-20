using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Perenthia.Web.Models;
using Radiance.Contract;

namespace Perenthia.Web.ActionResults
{
	public class ArmorialAvatarImageResult : ActionResult
	{
		public AvatarData Avatar { get; private set; }
		public string Size { get; set; }	
		public ArmorialAvatarImageResult(AvatarData avatar, string size)
		{
			this.Avatar = avatar;
			this.Size = size;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.Clear();
			context.HttpContext.Response.ContentType = "image/png";
			context.HttpContext.Response.Buffer = true;

			if (this.Avatar == null)
			{
				using (Image imgBlank = Image.FromFile(context.HttpContext.Server.MapPath("/common/media/avatar-blank.png")))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						imgBlank.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						ms.WriteTo(context.HttpContext.Response.OutputStream);
					}
				}
			}
			else
			{
				if (this.Size == "xl")
				{
					// XL Signature
					using (Image imgArmorial = Image.FromFile(context.HttpContext.Server.MapPath(String.Format("/common/images/armorial/armorial2-{0}-{1}.png", this.Avatar.Race, this.Avatar.Gender))))
					{
						using (Graphics g = Graphics.FromImage(imgArmorial))
						{
							// Name
							Font font = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold);
							g.DrawString(String.Format("{0} [{1}]", this.Avatar.Name, this.Avatar.Level),
								font, Brushes.White, 144, 2);

							// Race and Gender
							font = new Font(FontFamily.GenericSansSerif, 10);
							g.DrawString(String.Format("{0} {1}", this.Avatar.Race, this.Avatar.Gender),
								font, Brushes.White, 144, 34);

							// Location
							g.DrawString(String.Format("X:{0}, Y:{1}, Z:{2}", this.Avatar.X, this.Avatar.Y, this.Avatar.Z), font, Brushes.White, 144, 54);

							// Zone
							g.DrawString(this.Avatar.Zone, font, Brushes.White, 144, 74);

							// Is Online
							if (this.Avatar.IsOnline)
							{
								font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
								g.DrawString("[ ONLINE ]", font, Brushes.LimeGreen, 300, 34);
							}

							// Health
							int barWidth = 252;
							int barHeight = 10;
							double valPercentage = Convert.ToDouble((double)this.Avatar.Health * (100.0 / (double)this.Avatar.HealthMax));
							double dispPercentage = Convert.ToDouble(valPercentage * ((double)barWidth / 100.0));

							font = new Font(FontFamily.GenericSansSerif, 7);

							g.FillRectangle(Brushes.DarkGray, new Rectangle(144, 94, barWidth, barHeight));
							g.FillRectangle(new LinearGradientBrush(new Point(143, 94), new Point(396, 94), Color.DarkRed, Color.Red),
								new Rectangle(144, 94, (int)dispPercentage, barHeight));

							g.DrawString(String.Format("{0}/{1}", this.Avatar.Health, this.Avatar.HealthMax), font, Brushes.White, 146, 92);

							// Willpower
							valPercentage = Convert.ToDouble((double)this.Avatar.Willpower * (100.0 / (double)this.Avatar.WillpowerMax));
							dispPercentage = Convert.ToDouble(valPercentage * ((double)barWidth / 100.0));

							g.FillRectangle(Brushes.DarkGray, new Rectangle(144, 114, barWidth, barHeight));
							g.FillRectangle(new LinearGradientBrush(new Point(143, 114), new Point(396, 114), Color.DarkBlue, Color.Blue),
								new Rectangle(144, 114, (int)dispPercentage, barHeight));

							g.DrawString(String.Format("{0}/{1}", this.Avatar.Willpower, this.Avatar.WillpowerMax), font, Brushes.White, 146, 112);

							using (MemoryStream ms = new MemoryStream())
							{
								imgArmorial.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
								ms.WriteTo(context.HttpContext.Response.OutputStream);
							}
						}
					}
				}
				else
				{
					// Standard Image

					using (Image imgArmorial = Image.FromFile(context.HttpContext.Server.MapPath(String.Format("/common/images/armorial/armorial1-{0}-{1}.png", this.Avatar.Race, this.Avatar.Gender))))
					{
						using (Graphics g = Graphics.FromImage(imgArmorial))
						{
							// Name
							Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
							g.DrawString(String.Format("{0} [{1}]", this.Avatar.Name, this.Avatar.Level),
								font, Brushes.White, 2, 2);

							// Race and Gender
							if (!String.IsNullOrEmpty(this.Avatar.HouseholdName))
							{
								font = new Font(FontFamily.GenericSansSerif, 9);
								g.DrawString(String.Format("{0} - {1} ({2})", this.Avatar.HouseholdName, this.Avatar.RankName, this.Avatar.RankOrder),
									font, Brushes.White, 2, 20);
							}

							//// Is Online
							//if (this.Avatar.IsOnline)
							//{
							//    font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
							//    g.DrawString("[ ONLINE ]", font, Brushes.LimeGreen, 180, 26);
							//}

							// Location
							//g.DrawString(String.Format("X:{0}, Y:{1}, Z:{2}", this.Avatar.X, this.Avatar.Y, this.Avatar.Z), font, Brushes.White, 2, 46);

							// Zone
							//g.DrawString(this.Avatar.Zone, font, Brushes.White, 2, 34);

							using (MemoryStream ms = new MemoryStream())
							{
								imgArmorial.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
								ms.WriteTo(context.HttpContext.Response.OutputStream);
							}
						}
					}
				}
			}
		}
	}
}
