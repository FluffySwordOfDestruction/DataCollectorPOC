﻿using PackageManager.Compression;
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

        public string SerializeData(IEnumerable<DataTable> data)
        {
			return new DataSerializer(_serializationStrategy).Serialize(data);
        }

		public string SerializeData(Package data)
		{
			return new DataSerializer(_serializationStrategy).Serialize(data);
		}

		public byte[] Compress(string jsonPayloadData)
		{
			return new DataCompressor(_compressionStrategy).Compress(jsonPayloadData);
		}

		public Package Pack(byte[] data, EncryptionAlgorithmType encryptionAlgType, PayloadDataType dataType, DateTime timeStamp)
        {
            return new Package
            {
                DataType = dataType,
                EncryptionAlgorithm = encryptionAlgType,
                Retention = -1,
                TimeStamp = timeStamp,
                Payload = data
            };
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