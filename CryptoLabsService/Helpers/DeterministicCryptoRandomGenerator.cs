using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoLabsService.Helpers
{
    public class DeterministicCryptoRandomGenerator : RandomNumberGenerator
    {
        RandomNumberGenerator trueRng;
        Aes aes;
        bool useEntropy;
        long currentcounter;

        public DeterministicCryptoRandomGenerator(byte[] seed, bool useEntropy)
        {
            this.useEntropy = useEntropy;
            if (useEntropy)
            {
                this.trueRng = RandomNumberGenerator.Create();
            }
            else
            {
                this.aes = Aes.Create();
                if (seed.Length != this.aes.KeySize / 8)
                {
                    throw new InvalidOperationException();
                }
                this.aes.Mode = CipherMode.ECB;
                this.aes.Key = seed;
            }
        }

        public override void GetBytes(byte[] data, int offset, int count)
        {
            if (this.useEntropy)
            {
                this.trueRng.GetBytes(data, offset, count);
                return;
            }
            if (data.Length - offset < count)
            {
                throw new InvalidOperationException();
            }

            var iterationCount = offset / (this.aes.KeySize / 8);

            var encryptor = this.aes.CreateEncryptor();
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        for (long i = this.currentcounter; i < iterationCount + this.currentcounter; i++, this.currentcounter++)
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(BitConverter.GetBytes(i));

                            var encrypted = msEncrypt.ToArray();
                            Array.Copy(data, 0, encrypted, i * this.aes.BlockSize, this.aes.BlockSize);
                        }
                    }
                }
            }
        }

        public override void GetBytes(byte[] data)
        {
            this.GetBytes(data, 0, data.Length);
        }
        public override void GetNonZeroBytes(byte[] data)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            this.aes?.Dispose();
            this.trueRng?.Dispose();
            base.Dispose(disposing);
        }
    }
}
