using PackageManager.Compression.CompressionStrategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PackageManager.Compression
{
	internal class DataCompressor
	{
		private ICompressionStrategy _compressionStrategy;

		public DataCompressor(ICompressionStrategy compressionStrategy)
		{
			_compressionStrategy = compressionStrategy;
		}

		public string Decompress(byte[] data)
		{
			return _compressionStrategy.Decompress(data);
		}

		public byte[] Compress(string data)
		{
			return _compressionStrategy.Compress(data);
		}

		public IEnumerable<byte[]> Compress(IEnumerable<string> data)
		{
			foreach (string item in data)
				yield return _compressionStrategy.Compress(item);
		}
	}
}
