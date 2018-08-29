namespace CryptoLabsService.Managers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    using CryptoLabsService.Crypto;
    using CryptoLabsService.Helpers;

    public class StreamCipherIntegrityManager 
    {
        public byte[] EncryptCrt(byte[] data, byte[] seed, bool useEntropy = true, bool includeCtr = true)
        {
            byte[] ciphertext;
            byte[] ctr = new byte[16];

            var rand = new DeterministicCryptoRandomGenerator(seed, useEntropy);
            var keyRand = new DeterministicCryptoRandomGenerator(seed, false);

            rand.GetBytes(ctr);

            using (var aesAlg = new Aes128CounterMode(ctr))
            {
                var keyBytes = new byte[aesAlg.KeySize / 8];
                keyRand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                aesAlg.Key = keyBytes;

                // Create a crypto stream to perform encryption
                using (ICryptoTransform ecryptor = aesAlg.CreateEncryptor())
                {
                    // write encrypted bytes to memory
                    ciphertext = TransformHelper.PerformCryptography(ecryptor, data);
                }
            }

            if (!includeCtr)
            {
                return ciphertext;
            }
            var result = new byte[ctr.Length + ciphertext.Length];
            Array.Copy(ctr, 0, result, 0, ctr.Length);
            Array.Copy(ciphertext, 0, result, ctr.Length, ciphertext.Length);

            // Return the encrypted bytes from the memory stream. 
            return result;
        }

        public byte[] DecryptCtr(byte[] ciphertext, byte[] seed, byte[] ctr = null)
        {
            if (ciphertext.Length < 16 && ctr == null)
            {
                throw new Exception("CT len should be >= 16 bytes!");
            }

            if (ctr != null && ctr.Length != 16)
            {
                throw new Exception("CTR shoul be 16 bytes!");
            }

            byte[] encrypted;
            if (ctr == null)
            {
                ctr = ciphertext.Take(16).ToArray();
                encrypted = ciphertext.Skip(16).ToArray();
            }
            else
            {
                encrypted = ciphertext;
            }

            var keyRand = new DeterministicCryptoRandomGenerator(seed, false);

            byte[] result;

            using (var aesAlg = new Aes128CounterMode(ctr))
            {
                var keyBytes = new byte[aesAlg.KeySize / 8];
                keyRand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                aesAlg.Key = keyBytes;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                {
                    result = TransformHelper.PerformCryptography(decryptor, encrypted);
                }
            }

            return result;
        }
    }
}