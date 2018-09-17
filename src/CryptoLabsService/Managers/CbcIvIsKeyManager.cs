namespace CryptoLabsService.Managers
{
    using System;
    using System.Security.Cryptography;

    using CryptoLabsService.Helpers;

    public class CbcIvIsKeyManager
    {
        private const int AesKeySize = 128;

        public byte[] EncryptCbc(byte[] data, byte[] seed, bool useEntropy = false, bool includeIv = false)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, useEntropy))
            {
                byte[] iv;
                byte[] ciphertext;
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = AesKeySize;
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);

                    iv = keyBytes;

                    aesAlg.Key = keyBytes;
                    aesAlg.Padding = PaddingMode.None;

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

        public byte[] DecryptCbc(byte[] ciphertext, byte[] seed)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                byte[] iv;
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = AesKeySize;
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);

                    iv = keyBytes;

                    aesAlg.Key = keyBytes;

                    aesAlg.IV = iv;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.Zeros;

                    // Create the streams used for encryption. 
                    // Open a new memory stream to write the encrypted data to
                    // Create a crypto stream to perform encryption
                    using (var ecryptor = aesAlg.CreateDecryptor())
                    {
                        // write encrypted bytes to memory
                        return TransformHelper.PerformCryptography(ecryptor, ciphertext);
                    }
                }
            }
        }
    }
}