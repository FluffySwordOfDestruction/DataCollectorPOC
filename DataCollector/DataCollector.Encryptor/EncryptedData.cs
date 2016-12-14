using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCollector.Encryptor
{
    public class EncryptedData
    {
        public byte[] Data { get; set; }
        public byte[] Key { get; set; }
        public byte[] Vector { get; set; }
    }
}
