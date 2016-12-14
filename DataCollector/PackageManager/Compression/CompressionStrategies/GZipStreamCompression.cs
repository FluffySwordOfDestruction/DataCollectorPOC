using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PackageManager.Compression.CompressionStrategies
{
	public class GZipStreamCompression : ICompressionStrategy
	{
		public byte[] Compress(string data)
		{
			var bytes = Encoding.UTF8.GetBytes(data);

			using (var msi = new MemoryStream(bytes))
			using (var mso = new MemoryStream())
			{
				using (var gs = new GZipStream(mso, CompressionMode.Compress))
				{
					msi.CopyTo(gs);
				}

				return mso.ToArray();
			}
		}

		public string Decompress(byte[] bytes)
		{
			using (var msi = new MemoryStream(bytes))
			using (var mso = new MemoryStream())
			{
				using (var gs = new GZipStream(msi, CompressionMode.Decompress))
				{
					gs.CopyTo(mso);
				}

				return Encoding.UTF8.GetString(mso.ToArray());
			}
		}
	}
}
