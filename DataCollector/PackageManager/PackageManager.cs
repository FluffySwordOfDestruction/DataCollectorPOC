using PackageManager.Compression;
using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization;
using PackageManager.Serialization.SerializationStrategies;
using System;
using System.Collections.Generic;
using System.Data;

namespace PackageManager
{
	public class PackageManager
	{
		ISerializationStrategy _serializationStrategy;
		ICompressionStrategy _compressionStrategy;

		public PackageManager(ISerializationStrategy serializationStrategy, ICompressionStrategy compressionStrategy)
		{
			_serializationStrategy = serializationStrategy;
			_compressionStrategy = compressionStrategy;
		}

		public Package Pack(IEnumerable<DataTable> data)
		{
			DataSerializer serializer = new DataSerializer(_serializationStrategy);
			DataCompressor compressor = new DataCompressor(_compressionStrategy);

			var turbinesDataJson = serializer.Serialize(data);
			var turbinesDataBinary = compressor.Compress(turbinesDataJson);

			return new Package
			{
				DataType = PayloadDataType.Data10Minutes,
				EncryptionAlgorithm = EncryptionAlgorithmType.AES,
				Retention = -1,
				TimeStamp = DateTime.Now,
				Payload = turbinesDataBinary
			};
		}

		public string Unpack(byte[] payload)
		{
			DataCompressor compressor = new DataCompressor(_compressionStrategy);
			return compressor.Decompress(payload);
		}

		public byte[] ToBinaryFormat(Package package)
		{
			return PackageBinaryFormatter.ToBinaryFormat(package);
		}

		public Package FromBinaryFormat(byte[] packageBinary)
		{
			return PackageBinaryFormatter.FromBinaryFormat(packageBinary);
		}
	}
}