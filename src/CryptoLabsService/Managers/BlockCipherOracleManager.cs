namespace CryptoLabsService.Managers
{
    using System;
    using System.Security.Cryptography;

    using CryptoLabsService.Helpers;

    public class BlockCipherOracleManager 
    {
        private const int AesBlockSize = 16;

        public byte[] EncryptCbc(byte[] data, byte[] seed, bool useEntropy = true, bool includeIv = true)
        {

            byte[] iv = new byte[AesBlockSize];
            using (var rand = new DeterministicCryptoRandomGenerator(seed, useEntropy))
            {
                byte[] ciphertext;
                using (var aesAlg = Aes.Create())
                {
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                    aesAlg.Key = keyBytes;

                    rand.GetBytes(iv);

                    aesAlg.IV = iv;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.Zeros;

                    // Create the streams used for encryption. 
                    // Open a new memory stream to write the encrypted data to
                    // Create a crypto stream to perform encryption
                    using (ICryptoTransform ecryptor = aesAlg.CreateEncryptor())
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

        public byte[] EncryptEcb(byte[] data, byte[] seed, bool useEntropy = true)
        {
            byte[] ciphertext;
            using (var rand = new DeterministicCryptoRandomGenerator(seed, useEntropy))
            {
                using (var aesAlg = Aes.Create())
                {
                    var keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);
                    aesAlg.Key = keyBytes;

                    aesAlg.Mode = CipherMode.ECB;

                    // Create a crypto stream to perform encryption
                    using (ICryptoTransform ecryptor = aesAlg.CreateEncryptor())
                    {
                        // write encrypted bytes to memory
                        ciphertext = TransformHelper.PerformCryptography(ecryptor, data);
                    }
                }
            }

            return ciphertext;
        }

        public byte[] EncryptOracle(byte[] data, byte[] seed, bool useEntropy = true)
        {
            var paddedData = this.PadData(data, seed, useEntropy);

            var coinFlip = seed[seed.Length - 1] % 2 == 0;
            if (coinFlip)
            {
                 return this.EncryptEcb(paddedData, seed, useEntropy);
            }
            return this.EncryptCbc(paddedData, seed, useEntropy, false);
        }

        private byte[] PadData(byte[] data, byte[] seed, bool useEntropy = true)
        {
            byte[] result;
            using (var rand = new DeterministicCryptoRandomGenerator(seed, useEntropy))
            {
                byte[] randomData = new byte[2];
                rand.GetBytes(randomData, 0, randomData.Length);

                var firstOffsetValue = BitConverter.ToUInt32(data, 0);
                var secondOffsetValue = BitConverter.ToUInt32(data, 1);

                var firstOffSet = (int)(firstOffsetValue % 5 + 5);
                var secondOffSet = (int)(secondOffsetValue % 5 + 5);

                result = new byte[data.Length + firstOffSet + secondOffSet];

                rand.GetBytes(result, 0, firstOffSet);
                Array.Copy(data, 0, result, firstOffSet, data.Length);
                rand.GetBytes(result, data.Length + firstOffSet, secondOffSet);
            }

            return result;
        }
    }
}