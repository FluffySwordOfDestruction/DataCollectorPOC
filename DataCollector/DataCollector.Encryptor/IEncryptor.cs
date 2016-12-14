using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector.Encryptor
{
    public interface IEncryptor
    {
        EncryptedData EncryptData(byte[] data);
    }
}
