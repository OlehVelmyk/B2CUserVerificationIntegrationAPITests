using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Serilog;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common
{
    internal class Rc2EncryptProvider : IEncryptProvider
    {
        private readonly ILogger _logger;
        private readonly byte[] _keyBytes = Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");
        private readonly byte[] _vectorBytes = Encoding.ASCII.GetBytes("???0????");

        public Rc2EncryptProvider(ILogger logger)
        {
            this._logger = logger.ForContext<Rc2EncryptProvider>();
        }

        public string EncryptText(string data)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                // Create a new RC2 object.
                RC2 RC2alg = RC2.Create();

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    RC2alg.CreateEncryptor(_keyBytes, _vectorBytes),
                    CryptoStreamMode.Write);

                // Convert the passed string to a byte array.
                byte[] toEncrypt = Encoding.Unicode.GetBytes(data);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return Convert.ToBase64String(ret);
            }
            catch (CryptographicException ex)
            {
                this._logger.Error(ex, "Exception while encrypt data");
                return null;
            }
        }

        public string DecryptText(string data)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                var dataArr = Convert.FromBase64String(data);
                MemoryStream msDecrypt = new MemoryStream(dataArr);

                // Create a new RC2 object.
                RC2 RC2alg = RC2.Create();

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    RC2alg.CreateDecryptor(_keyBytes, _vectorBytes),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[dataArr.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                //Convert the buffer into a string and return it.
                return Encoding.Unicode.GetString(fromEncrypt).TrimEnd('\0');
            }
            catch (CryptographicException ex)
            {
                this._logger.Error(ex, "Exception while decrypt data");
                return null;
            }
        }
    }
}
