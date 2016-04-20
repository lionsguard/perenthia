using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;
using Radiance.Contract;
using System.Threading;
using Lionsguard;

namespace Perenthia.Utility
{
	public static class Depot
	{
		#region Cross Domain Policy Data
		public static readonly string ClientAccessPolicyData = @"
<?xml version=""1.0"" encoding=""utf-8"" ?>
<access-policy>
	<cross-domain-access>
		<policy>
			<allow-from http-request-headers=""*"">
				<domain uri=""*""/>
			</allow-from>
			<grant-to>
				<resource include-subpaths=""true"" path=""/""/>
				<socket-resource port=""4502-4532"" protocol=""tcp""/>
			</grant-to>
		</policy>
	</cross-domain-access>
</access-policy>
";

		public static readonly string CrossDomainPolicyData = @"
<?xml version=""1.0"" encoding=""utf-8"" ?>
<cross-domain-policy>
	<allow-http-request-headers-from domain=""*"" headers=""*""/>
</cross-domain-policy>
";
		#endregion

		#region Map Data
		public static RdlTagCollection GetMapNames()
		{
			RdlTagCollection tags = new RdlTagCollection();
			foreach (var detail in Game.Server.World.Map.MapDetails.Values)
			{
				RdlTag tag = new RdlTag("MAP", "MAP");
				tag.Args.Add(detail.Name);
				tag.Args.Add(detail.Width);
				tag.Args.Add(detail.Height);
				tag.Args.Add(detail.Key.StartX);
				tag.Args.Add(detail.Key.StartY);
				tag.Args.Add(detail.Key.EndX);
				tag.Args.Add(detail.Key.EndY);
				tags.Add(tag);
			}
			return tags;
		}
		public static MapChunk GetMapChunk(string mapName, int startX, int startY, bool includeActors)
		{
			RdlTagCollection tags = new RdlTagCollection();
			List<Place> places = Game.Server.World.Map.GetMap(mapName, startX, startY, Game.Server.World.Map.DefaultWidth, Game.Server.World.Map.DefaultHeight);
			if (places.Count > 0)
			{
				for (int i = 0; i < places.Count; i++)
				{
					tags.AddRange(places[i].ToRdl());

					if (includeActors)
					{
						// Send down actors in the places as well.
						foreach (var actor in places[i].Children)
						{
							if (actor.ObjectType != ObjectType.Player)
							{
								tags.AddRange(actor.ToRdl());
							}
						}
					}
				}
			}
			return new MapChunk 
			{
				MapName = mapName, 
				StartX = startX, 
				StartY = startY, 
				Tags = tags.ToString() 
			};
		}
		#endregion

		#region Error Data
		public static void SubmitErrorData(string remoteHost, string errorData)
		{
			Lionsguard.Log.Write(String.Format("UI ERROR DATA:{0}From:{1}{0}Error:{2}",
				Environment.NewLine, remoteHost, errorData), true);
		}
		#endregion
	}
}
