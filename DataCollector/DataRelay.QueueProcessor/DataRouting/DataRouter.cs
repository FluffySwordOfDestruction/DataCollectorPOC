using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization.SerializationStrategies;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Serialization;

namespace DataRelay.QueueProcessor.DataRouting
{
	public class DataRouter
	{
		public static bool Route(Package package)
		{
			try
			{
				RoutingInfo routingInfo = GetRoutingInfo();
				byte[] packageBinary = PrepareForTransport(package);

				foreach (var endpointDetails in routingInfo.Endpoints.Where(e => e.DataType.Contains(package.DataType)))
				{
					Uri endpointUri = new Uri(endpointDetails.Address);
					EndpointAddress endpointAddr = new EndpointAddress(endpointUri);

					DataRelayWcf.DataRelayServiceClient client = new DataRelayWcf.DataRelayServiceClient("BasicHttpBinding_IDataRelay", endpointAddr);
					client.Send(packageBinary);
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		private static RoutingInfo GetRoutingInfo()
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RoutingInfo.xml");

			StreamReader reader = new StreamReader(path);
			XmlSerializer serializer = new XmlSerializer(typeof(RoutingInfo));
			RoutingInfo routingInfo = (RoutingInfo)serializer.Deserialize(reader);
			reader.Close();

			return routingInfo;
		}

		private static byte[] PrepareForTransport(Package package)
		{
			PackageManager.PackageManager pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());
			return pm.ToBinaryFormat(package);
		}
	}
}
