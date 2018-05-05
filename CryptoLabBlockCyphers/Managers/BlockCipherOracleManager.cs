using CryptoLabBlockCiphers.Interfaces;

namespace CryptoLabBlockCyphers.Interfaces
{
    public class BlockCipherOracleManager : IBlockCipherOracleManager
    {
        public byte[] EncryptCbc(byte[] data, byte[] seed, bool useEntropy = true)
        {
            throw new System.NotImplementedException();
        }

        public byte[] EncryptEcb(byte[] data, byte[] seed, bool useEntropy = true)
        {
            throw new System.NotImplementedException();
        }

        public byte[] EncryptOracle(byte[] data, byte[] seed, bool useEntropy = true)
        {
            throw new System.NotImplementedException();
        }
    }
}