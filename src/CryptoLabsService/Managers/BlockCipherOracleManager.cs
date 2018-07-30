using CryptoLabBlockCiphers.Interfaces;
using CryptoLabsService.Helpers;
using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoLabBlockCyphers.Interfaces
{
    public class BlockCipherOracleManager : IBlockCipherOracleManager
    {
        public byte[] EncryptCbc(byte[] data, byte[] seed, bool useEntropy = true, bool includeIv = true)
        {
            byte[] cipherTest;
            byte[] IV;
            DeterministicCryptoRandomGenerator rand = new DeterministicCryptoRandomGenerator(seed, useEntropy);

            using (Aes aesAlg = Aes.Create())
            {
                var keyBytes = new byte[aesAlg.KeySize / 8];
                rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                aesAlg.Key = keyBytes;

                if (useEntropy)
                {
                    aesAlg.GenerateIV();
                }
                else
                {
                    aesAlg.IV = new byte[aesAlg.BlockSize / 8];
                    if (seed.Length < aesAlg.BlockSize / 8)
                    {
                        Array.Copy(seed, aesAlg.IV, seed.Length);
                    }
                    else
                    {
                        Array.Copy(seed, aesAlg.IV, aesAlg.BlockSize / 8);
                    }
                }

                IV = aesAlg.IV;
                aesAlg.Mode = CipherMode.CBC;

                // Create the streams used for encryption. 
                // Open a new memory stream to write the encrypted data to
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create a crypto stream to perform encryption
                    using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // write encrypted bytes to memory
                        cs.Write(data, 0, data.Length);
                    }
                    cipherTest = ms.ToArray();
                }
            }
            if (!includeIv)
            {
                return cipherTest;                
            }
            var result = new byte[IV.Length + cipherTest.Length];
            Array.Copy(IV, 0, result, 0, IV.Length);
            Array.Copy(cipherTest, 0, result, IV.Length, cipherTest.Length);

            // Return the encrypted bytes from the memory stream. 
            return result;

        }

        public byte[] EncryptEcb(byte[] data, byte[] seed, bool useEntropy = true)
        {
            byte[] cipherTest;
            DeterministicCryptoRandomGenerator rand = new DeterministicCryptoRandomGenerator(seed, useEntropy);

            using (Aes aesAlg = Aes.Create())
            {
                var keyBytes = new byte[aesAlg.KeySize / 8];
                rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                aesAlg.Key = keyBytes;

                aesAlg.Mode = CipherMode.ECB;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, new byte[aesAlg.BlockSize/8]);

                // Create the streams used for encryption. 
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create a crypto stream to perform encryption
                    using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // write encrypted bytes to memory
                        cs.Write(data, 0, data.Length);
                    }
                    cipherTest = ms.ToArray();
                }
            }
            return cipherTest;
        }

        public byte[] EncryptOracle(byte[] data, byte[] seed, bool useEntropy = true)
        {
            var paddedData = this.PadData(data, seed, useEntropy);

            var coinFlip = seed[seed.Length - 1] % 2 == 0;
            if (coinFlip)
            {
                 return this.EncryptEcb(paddedData, seed, useEntropy);
            }
            else
            {
                return this.EncryptCbc(paddedData, seed, useEntropy, false);
            }
        }

        private byte[] PadData(byte[] data, byte[] seed, bool useEntropy = true)
        {
            DeterministicCryptoRandomGenerator rand;
            if (!useEntropy)
            {
                rand = new DeterministicCryptoRandomGenerator(seed, useEntropy);
            }
            else
            {
                rand = new DeterministicCryptoRandomGenerator(seed, useEntropy);
            }
            byte[] randomData = new byte[2];
            rand.GetBytes(randomData, 0, randomData.Length);

            var firstOffsetValue = BitConverter.ToUInt32(data, 0);
            var secondOffsetValue = BitConverter.ToUInt32(data, 1);

            var firstOffSet = (int)(firstOffsetValue % 5 + 5);
            var secondOffSet = (int)(secondOffsetValue % 5 + 5);

            var result = new byte[data.Length + firstOffSet + secondOffSet];

            rand.GetBytes(result, 0, firstOffSet);
            Array.Copy(data, 0, result, firstOffSet, data.Length);
            rand.GetBytes(result, data.Length + firstOffSet, secondOffSet);

            return result;
        }
    }
}