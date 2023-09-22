using System;
using System.Security.Cryptography;
using CryptoLabsService.Helpers;

namespace CryptoLabsService.Labs.CbcIvIsTime
{
    public class CbcIvIsTimeManager
    {
        public byte[] EncryptCbc(byte[] data, byte[] seed, bool useEntropy = true, bool includeIv = true)
        {
            byte[] ciphertext;
            var iv = this.GetIv();

            using (var rand = new DeterministicCryptoRandomGenerator(seed, useEntropy))
            {
                using (var aesAlg = Aes.Create())
                {
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                    aesAlg.Key = keyBytes;

                    aesAlg.IV = iv;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.Zeros;

                    // Create the streams used for encryption.
                    // Open a new memory stream to write the encrypted data to
                    // Create a crypto stream to perform encryption
                    using (var ecryptor = aesAlg.CreateEncryptor())
                    {
                        // write encrypted bytes to memory
                        ciphertext = TransformHelper.PerformCryptography(ecryptor, data);
                    }
                }

                if (!includeIv)
                {
                    return ciphertext;
                }

                var result = new byte[iv.Length + ciphertext.Length];
                Array.Copy(iv, 0, result, 0, iv.Length);
                Array.Copy(ciphertext, 0, result, iv.Length, ciphertext.Length);

                // Return the encrypted bytes from the memory stream.
                return result;
            }
        }

        /// <summary>
        /// Get iv based on unix time
        /// </summary>
        /// <returns></returns>
        public byte[] GetIv()
        {
            var iv = new byte[16];

            var unixTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            var unixTimeBytes = BitConverter.GetBytes(unixTimestamp);

            // Copy to the end
            unixTimeBytes.CopyTo(iv, iv.Length - unixTimeBytes.Length);

            return iv;
        }
    }
}