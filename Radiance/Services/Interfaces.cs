using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web;

namespace Radiance.Services
{
	[ServiceContract(Namespace = "Radiance.Services", CallbackContract = typeof(IGameClient))]
	public interface IGameServer
	{
		[OperationContract]
		void ProcessCommand(Message message);
    }
    
    /// <summary>
    /// Defines a cllas that provides a client access policy file.
    /// </summary>
    [ServiceContract]
    public interface IPolicyProvider
    {
        /// <summary>
        /// Gets the stream containing the information from the client access policy xml file.
        /// </summary>
        /// <returns>The client access policy xml file stream.</returns>
        [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
		Stream GetSilverlightPolicy();

		/// <summary>
		/// Gets the stream containing the information from the client access policy xml file.
		/// </summary>
		/// <returns>The client access policy xml file stream.</returns>
		[OperationContract, WebGet(UriTemplate = "/crossdomain.xml")]
		Stream GetFlashPolicy();
    }

	[ServiceContract(Namespace = "Radiance.Services")]
	public interface IGameClient
    {
        /// <summary>
        /// Begins an asyncronous call to receive a Message sent from the server.
        /// </summary>
        /// <param name="message">The Message sent from the server.</param>
        /// <param name="callback">The AsyncCallback delegate used to handle the callback response.</param>
        /// <param name="state">A custom object containing user specific state.</param>
        [OperationContract(IsOneWay = true, AsyncPattern = true)]
        IAsyncResult BeginReceive(Message message, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asyncronous call to receive a Message sent from the server.
        /// </summary>
        /// <param name="result">The IAsyncResult of the current call.</param>
        void EndReceive(IAsyncResult result);
	}
}
