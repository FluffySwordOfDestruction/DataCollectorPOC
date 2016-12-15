using DataCollector.Encryptor;
using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization.SerializationStrategies;
using System;

namespace DataRelay.QueueProcessor
{
	public class DataProcessor
	{
		public static bool RecieveData(byte[] packageByte)
		{
			var package = PreparePackage(packageByte);

			// Do not decrypt payload for now
			//byte[] payload = DecryptPackagePayload(package);

			FileManager.SaveToFile(package);

			SendToQueue(package);

			return true;
		}

		private static byte[] DecryptPackagePayload(Package package)
		{
			AESEncryptor decrypt = new AESEncryptor();
			return decrypt.DecryptData(package.Payload, package.EncryptedKey, package.EncryptedIV);
		}

		private static Package PreparePackage(byte[] package)
		{
			PackageManager.PackageManager pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());
			return pm.FromBinaryFormat(package);
		}

		private static void SendToQueue(Package package)
		{
			QueueManager.Enqueue(package);
		}
	}
}
