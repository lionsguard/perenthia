using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Contract
{
	public class Constants
	{
		public const string ServiceNamespace = "urn:Lionsguard:Radiance:2010:01:schemas";

		public const string GameServiceProcess = "urn:Lionsguard:Radiance:2010:01:schemas/IGameService/Process";
		public const string GameServiceReceive = "urn:Lionsguard:Radiance:2010:01:schemas/IGameService/Receive";

		public const string DepotServiceGetMapNames = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapNames";
		public const string DepotServiceGetMapChunk = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapChunk";
		public const string DepotServiceGetMapNamesResponse = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapNamesResponse";
		public const string DepotServiceGetMapChunkResponse = "urn:Lionsguard:Radiance:2010:01:schemas/IDepotService/GetMapChunkResponse";
	}
}
