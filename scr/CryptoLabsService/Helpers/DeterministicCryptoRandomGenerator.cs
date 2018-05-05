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

            var iterationCount = (count / (this.aes.KeySize / 8)) + 1;

            using (var aesEncryptor = this.aes.CreateEncryptor())
            {
                long i;
                for (i = this.currentcounter; i < iterationCount + this.currentcounter; i++)
                {
                    byte[] encrypted = new byte[this.aes.BlockSize/8];
                    byte[] rawBlock = new byte[this.aes.BlockSize / 8];
                    byte[] encodedCounter = BitConverter.GetBytes(i);
                    Array.Copy(encodedCounter, rawBlock, encodedCounter.Length);
                    // Transform one block
                    aesEncryptor.TransformBlock(rawBlock, 0, 16, encrypted, 0);

                    if (count - (i+1 - this.currentcounter) * this.aes.BlockSize/8 > 0)
                    {
                        Array.Copy(encrypted, 0, data, (i - this.currentcounter) * this.aes.BlockSize/8 + offset, this.aes.BlockSize/8);
                    }
                    else
                    {
                        Array.Copy(encrypted, 0, data, (i - this.currentcounter) * this.aes.BlockSize/8 + offset, count - (i - this.currentcounter) * this.aes.BlockSize/8);
                    }
                }
                this.currentcounter += i;
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
