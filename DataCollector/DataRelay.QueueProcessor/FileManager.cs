using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization.SerializationStrategies;
using System;
using System.IO;

namespace DataRelay.QueueProcessor
{
	public class FileManager
	{
		public static void SaveToFile(Package package)
		{
			PackageManager.PackageManager pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());

			//string payload = pm.Unpack(package.Payload);
			string json = pm.SerializeData(package);
			string basePath = AppDomain.CurrentDomain.BaseDirectory;
			string fileName = Path.Combine(Path.GetDirectoryName(basePath), "RecievedPackages", package.DataType.ToString() + "_" + package.TimeStamp.ToString("yyyy-MM-dd HH_mm_ss_fff tt") + "_tick_" + DateTime.UtcNow.Ticks + ".json");

			File.WriteAllText(fileName, json);
		}
	}
}
