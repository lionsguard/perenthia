using System.ServiceModel;
using System.ServiceModel.Web;

namespace Radiance.Contract
{
	[ServiceContract(Namespace = Constants.ServiceNamespace)]
	public interface ISimplexGameService
	{
		[OperationContract]
		[WebGet]
		string Process(string data);
	}
}
