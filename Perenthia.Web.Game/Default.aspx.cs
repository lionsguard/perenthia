using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.IO;

namespace Perenthia.Web
{
	public partial class _Default : System.Web.UI.Page
	{
		private string _initParams = String.Empty;
		public string InitParams
		{
			get { return _initParams; }
		}

		public bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var authKey = Request.Params["authKey"];

			_initParams = String.Format("LoaderSourceList=/ClientBin/Perenthia.xap,authKey={0},version={1},mediaUri={2},servicesRootUri={3},gameService={4},depotService={5},armorialService={6},gameServerPort={7}",
				authKey,
				typeof(Character).Assembly.GetName().Version.ToString(3),
				WebConfigurationManager.AppSettings["MediaUri"],
				GetCurrentUri(),
				"Services/HttpGameService.ashx",
				"Services/DepotService.svc",
				"Services/ArmorialService.svc",
				WebConfigurationManager.AppSettings["GameServerPort"]);

			// YMLP
			//<form method="post" action="http://ymlp.com/subscribe.php?YMLPID=gejusymgmgj" target="_blank">
			//<table border="0" align="center" cellspacing="0" cellpadding="5">
			//<tr><td colspan="2"><font size="2" face="verdana,geneva">Fill out your e-mail address<br />to receive our newsletter!</font></td></tr>
			//<tr><td valign="top"><font size="2" face="verdana,geneva">E-mail address:</font></td><td valign="top"><input type="text" name="YMP0" size="20" /></td></tr>
			//<tr><td colspan="2"><input type="submit" value="Submit"  />&nbsp;</td></tr>
			//</table>
			//</form>


			//// Generate races, terrain and skills data files.
			//var races = Game.Server.World.Races.ToRdl();
			//var skills = Game.Server.World.Skills.ToRdl();
			//var groups = Game.Server.World.SkillGroups.ToRdl();
			//var terrain = Game.Server.World.GetRdlTerrain();

			//var path = Server.MapPath("");

			//WriteFile(Path.Combine(path, "races.txt"), new Radiance.Markup.RdlTagCollection(races));
			//WriteFile(Path.Combine(path, "terrain.txt"), new Radiance.Markup.RdlTagCollection(terrain));
			//WriteFile(Path.Combine(path, "skills.txt"), new Radiance.Markup.RdlTagCollection(skills));
			//WriteFile(Path.Combine(path, "skillgroups.txt"), new Radiance.Markup.RdlTagCollection(groups));

		}
		private void WriteFile(string fileName, Radiance.Markup.RdlTagCollection tags)
		{
			using (var sw = new StreamWriter(fileName, false))
			{
				sw.WriteLine(tags.ToString());
			}
		}

		private string GetCurrentUri()
		{
			if (Request.Url.IsLoopback)
			{
				return String.Format("http://127.0.0.1.:{0}", Request.Url.Port);
			}
			return String.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
		}
	}
}
