using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace DataCollector.Encryptor
{
    public class AESEncryptor : IEncryptor
    {
        public EncryptedData EncryptData(byte[] data)
        {
            EncryptedData encryptedData = new EncryptedData();
           
                using (Aes myAes = Aes.Create())
                {

                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = Encrypt(data, myAes.Key, myAes.IV);
                    encryptedData.Data = encrypted;
                    encryptedData.Key = myAes.Key;
                    encryptedData.Vector = myAes.IV;

                    // Decrypt the bytes to a string.
                    //byte[] roundtrip = DecryptStringFromBytes_Aes(encryptedData.Data, encryptedData.Key, encryptedData.Vector);

                    //bool isTheSameAfterDecrypt = data.SequenceEqual(roundtrip);
                    //if (!isTheSameAfterDecrypt)
                    //    Trace.WriteLine("decrypted data is not the same as original!");
                   
                }

            return encryptedData;
        }

        byte[] Encrypt(byte[] plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {

                        //Write all data to the stream.
                        csEncrypt.Write(plainText, 0, plainText.Length);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        public byte[] DecryptData(byte[] encryptedData, byte[] key, byte[] IV)
        {
            return Decrypt(encryptedData, key, IV);
        }

        byte[] Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            byte[] plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        var t = msDecrypt.ToArray();     
                        using(MemoryStream stream = new MemoryStream())
                        {
                            csDecrypt.CopyTo(stream);
                            plaintext = stream.ToArray();
                        }                 
                    }
                }

            }

            return plaintext;

        }
    }
}
