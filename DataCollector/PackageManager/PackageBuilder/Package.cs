using System;

namespace PackageManager.PackageBuilder
{
	public class Package
	{
		public PayloadDataType DataType;
		public int Retention;
		public DateTime TimeStamp;
        public EncryptionAlgorithmType EncryptionAlgorithm;
        public byte[] EncryptedKey;
        public byte[] EncryptedIV;
		public byte[] Payload;
	}
}
