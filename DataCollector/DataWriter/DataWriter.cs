using PackageManager.Compression.CompressionStrategies;
using PackageManager.PackageBuilder;
using PackageManager.Serialization.SerializationStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCollector.Encryptor;
using System.IO;

namespace DataWriter
{
    public class DataWriter : IDataWriter
    {
        PackageManager.PackageManager _pm = new PackageManager.PackageManager(new JsonNet(), new GZipStreamCompression());
        public bool ProcessData(byte[] data)
        {
            var package = GetPackage(data);
            var decryptedData = DecrypData(package.Payload, package.EncryptedKey, package.EncryptedIV);
            var plainData = DeserializeData(decryptedData);
            return SaveToFile(plainData, package.DataType, package.TimeStamp);
        }

        private Package GetPackage(byte[] data)
        {
            return _pm.FromBinaryFormat(data);
        }

        private byte[] DecrypData(byte[] encryptedData, byte[] key, byte[] IV)
        {
            AESEncryptor decryptor = new AESEncryptor();
            return decryptor.DecryptData(encryptedData, key, IV);
        }

        private string DeserializeData(byte[] data)
        {
            return _pm.Unpack(data);
        }

        private bool SaveToFile(string data, PayloadDataType dataType, DateTime timeStamp)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = Path.Combine(Path.GetDirectoryName(basePath), "ReceivedPackages", dataType.ToString() + "_" + timeStamp.ToString("yyyy-MM-dd HH_mm_ss_fff tt") + "_tick_" + DateTime.UtcNow.Ticks + ".json");

            try
            {
                File.WriteAllText(fileName, data);
            }
            catch
            {
                return false;
            }
            return  true;
        }


    }
}
