using System.ServiceModel;
namespace DataRelay.WcfHost
{
	[ServiceContract]
	public interface IDataRelayService
	{
		[OperationContract]
		bool Send(byte[] package);

		[OperationContract]
		bool KnockKnock(byte[] package);
	}
}
