namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/HmacTiming")]
    public class HmacTimingController : Controller
    {
        private const int AesBlockSize = 16;

        private readonly BlockCipherOracleManager blockCipherOracleManager;

        public HmacTimingController(BlockCipherOracleManager blockCipherOracleManager)
        {
            this.blockCipherOracleManager = blockCipherOracleManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/data={data}&mac={mac}&delay={delay}")]
        public string GetVerify(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string data,
            [FromRoute] string mac,
            [FromRoute] int delay)
        {
            delay = delay > 1000 ? 1000 : delay;

            var hash = SHA256.Create();

            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var macHash = new HMACSHA256(seed);
            var targetHash = macHash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + data));
            if (!this.InsecureCompareArrays(targetHash.Take(8).ToArray(), this.StringToByteArray(mac), delay))
            {
                return "INVALID_MAC";
            }

            return "Wellcome to secretNet!";
        }


        private bool InsecureCompareArrays(byte[] source, byte[] target, int delay)
        {

            var toSend = BitConverter.ToString(source).Replace("-", "");

            if (source.Length != target.Length)
            {
                return false;
            }

            for (var index = 0; index < target.Length; index++)
            {
                System.Threading.Thread.Sleep(delay);
                if (source[index] != target[index])
                {
                    return false;
                }
            }

            return true;
        }

        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}