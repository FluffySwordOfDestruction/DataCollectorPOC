using MsgPack.Serialization;
using System.IO;

namespace PackageManager.PackageBuilder
{
	public class PackageBinaryFormatter
	{
		public static byte[] ToBinaryFormat(Package package)
		{
			MemoryStream stream = new MemoryStream();

			var serializer = SerializationContext.Default.GetSerializer<Package>();
			serializer.Pack(stream, package);

			return stream.ToArray();
		}

		public static Package FromBinaryFormat(byte[] packageBinary)
		{
			MemoryStream stream = new MemoryStream();

			var serializer = SerializationContext.Default.GetSerializer<Package>();
			return serializer.Unpack(new MemoryStream(packageBinary));
		}
	}
}
