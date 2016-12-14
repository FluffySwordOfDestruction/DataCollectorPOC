using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization.SerializationStrategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DataCollector.Api.DataManager
{
	public class DataTransformator
	{
		public static object SerializeData(IEnumerable<DataTable> turbineData)
		{
			Stopwatch st = new Stopwatch();
			st.Start();

			PackageManager.PackageManager pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());

			// Step 1: serialize & compress
			Package package = pm.Pack(turbineData);

			// Step 2: to binary format
			byte[] packageBinary = pm.ToBinaryFormat(package);

			st.Stop();
			Console.WriteLine("Serializing, Compress, Binarize Data = " + st.ElapsedMilliseconds);

			return packageBinary;
		}
	}
}
