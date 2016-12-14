using System.Collections.Generic;

namespace PackageManager.Compression.CompressionStrategies
{
	public interface ICompressionStrategy
	{
		byte[] Compress(string data);
		string Decompress(byte[] json);
	}
}
