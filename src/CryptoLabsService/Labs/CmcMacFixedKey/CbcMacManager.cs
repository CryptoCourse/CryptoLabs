using System.Linq;
using System.Security.Cryptography;
using CryptoLabsService.Helpers;

namespace CryptoLabsService.Labs.CmcMacFixedKey
{
    public class CbcMacManager
    {
        private const int AesKeySize = 128;

        private const int AesBlockSize = 16;

        public byte[] ConputeMacWithKey(byte[] data, byte[] seed)
        {
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                byte[] iv;
                byte[] ciphertext;
                byte[] keyBytes;
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = AesKeySize;
                    keyBytes = new byte[aesAlg.KeySize / 8];
                    rand.GetBytes(keyBytes, 0, aesAlg.KeySize / 8);

                    iv = new byte[AesKeySize / 8];

                    aesAlg.Key = keyBytes;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    aesAlg.IV = iv;
                    aesAlg.Mode = CipherMode.CBC;

                    // Create the streams used for encryption.
                    // Open a new memory stream to write the encrypted data to
                    // Create a crypto stream to perform encryption
                    using (var ecryptor = aesAlg.CreateEncryptor())
                    {
                        // write encrypted bytes to memory
                        ciphertext = TransformHelper.PerformCryptography(ecryptor, data);
                    }
                }

                // return last block
                return ciphertext.Skip(ciphertext.Length - AesBlockSize).ToArray();
            }
        }

        public byte[] GetKey(byte[] seed)
        {
            byte[] keyBytes = new byte[AesKeySize / 8];
            using (var rand = new DeterministicCryptoRandomGenerator(seed, false))
            {
                rand.GetBytes(keyBytes, 0, keyBytes.Length);
            }

            return keyBytes;
        }
    }
}