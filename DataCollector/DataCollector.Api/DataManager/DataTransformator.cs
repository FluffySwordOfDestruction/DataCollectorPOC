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
        static PackageManager.PackageManager _pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());

        public static byte[] SerializeData(IEnumerable<DataTable> turbineData)
        {
            Stopwatch st = new Stopwatch();
            st.Start();

            // Step 1: serialize
            var jsonResult = _pm.SerializeData(turbineData);

			// Step 2: compress
			var binaryResult = _pm.Compress(jsonResult);

			//// Step 3: to binary format
			//byte[] packageBinary = pm.ToBinaryFormat(package);

			st.Stop();
            Console.WriteLine("Serializing, Compress = " + st.ElapsedMilliseconds);

            return binaryResult;
        }


        public static Package PreaparePackage(byte[] serializedData, EncryptionAlgorithmType encryptionAlgType, PayloadDataType dataType, DateTime timeStamp )
        {
            return _pm.Pack(serializedData, encryptionAlgType, dataType, timeStamp);
        }

        public static byte[] GetBinary(Package package)
        {
            return _pm.ToBinaryFormat(package);
        }
    }
}
