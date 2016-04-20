using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Radiance.Contract;
using Perenthia.ServiceModel;
using Perenthia.Utility.ServiceModel;

namespace Perenthia.Web.Services
{
	public class ArmorialServiceHostFactory : HttpServiceFactory<ArmorialService, IArmorialService> { }

	public class GameServiceHostFactory : HttpStaticServiceFactory<GameService, IGameService> { }

	public class DepotServiceHostFactory : HttpServiceFactory<DepotService, IDepotService> { }

	public class SimplexGameServiceFactory : WebServiceFactory<SimplexGameService, ISimplexGameService> { }
}
