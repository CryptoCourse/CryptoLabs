namespace CryptoLabBlockCiphers.Interfaces
{
    public interface IBlockCipherOracleManager
    {
        /// <summary>
        /// Зашифровать данные в режиме ECB
        /// </summary>
        /// <param name="data">Данные для зашифрования</param>
        /// <param name="seed">Seed для уникального задания</param>
        /// <param name="useEntropy">Использовать энтропию</param>
        /// <returns></returns>
        byte[] EncryptEcb(byte[] data, byte[] seed, bool useEntropy = true);

        /// <summary>
        /// Зашифровать данные в режиме ECB
        /// </summary>
        /// <param name="data">Данные для зашифрования</param>
        /// <param name="seed">Seed для уникального задания</param>
        /// <param name="useEntropy">Использовать энтропию</param>
        /// <returns></returns>
        byte[] EncryptCbc(byte[] data, byte[] seed, bool useEntropy = true, bool includeIv = true);

        /// <summary>
        /// Зашифровать данные в неизвестном режиме, режим определяется на основе seed
        /// </summary>
        /// <param name="data">Данные для зашифрования</param>
        /// <param name="seed">Seed для уникального задания</param>
        /// <param name="useEntropy">Использовать энтропию</param>
        /// <returns></returns>
        byte[] EncryptOracle(byte[] data, byte[] seed, bool useEntropy = true);
    }
}
