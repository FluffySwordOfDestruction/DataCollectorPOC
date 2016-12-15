using PackageManager.PackageBuilder;
using System;
using System.Xml.Serialization;

namespace DataRelay.QueueProcessor.DataRouting
{
	[Serializable(), XmlRoot("routingInfo")]
	public class RoutingInfo
	{
		[XmlArray("endpoints")]
		[XmlArrayItem("endpoint", typeof(EndpointDetails))]
		public EndpointDetails[] Endpoints { get; set; }
	}

	public class EndpointDetails
	{
		[XmlAttribute("address")]
		public string Address { get; set; }

		[XmlArray("dataTypes")]
		[XmlArrayItem("dataType", typeof(PayloadDataType))]
		public PayloadDataType[] DataType { get; set; }
	}
}
