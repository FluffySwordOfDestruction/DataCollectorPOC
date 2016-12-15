using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization.SerializationStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DataRelay.WcfHostTester
{
	class Program
	{
		static void Main(string[] args)
		{
			for (int i = 0; i < 1; i++)
			{
				Package package = new Package
				{
					DataType = PayloadDataType.Data10Minutes,
					EncryptionAlgorithm = EncryptionAlgorithmType.AES,
					Retention = -1,
					TimeStamp = DateTime.Now,
					Payload = new byte[3600]
				};

				PackageManager.PackageManager pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());
				byte[] packageBinary = pm.ToBinaryFormat(package);

				Console.WriteLine("Sending Package..." + i);

				DataRelayWcf.DataRelayServiceClient client = new DataRelayWcf.DataRelayServiceClient();
				var status = client.Send(packageBinary);

				Console.WriteLine("Sending Package...");

				//Thread.Sleep(100);
			}

			Console.ReadKey();
		}
	}
}
