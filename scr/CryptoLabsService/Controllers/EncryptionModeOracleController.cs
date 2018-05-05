using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoLabBlockCiphers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoLabBlockCyphers.Controllers
{
    [Route("api/EncryptionModeOracle")]
    public class EncryptionModeOracleController : Controller
    {
        IBlockCipherOracleManager blockCipherOracleManager;

        public EncryptionModeOracleController(IBlockCipherOracleManager blockCipherOracleManager)
        {
            this.blockCipherOracleManager = blockCipherOracleManager;
        }

        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpPost]
        [Route("{userId}/{challengeId}")]
        public string Post(
            [FromQuery] string userId,
            [FromQuery] string challengeId,
            [FromBody] string value)
        {
            SHA256 hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));
            var data = Convert.FromBase64String(value);
            var result = this.blockCipherOracleManager.EncryptOracle(data, seed);
            return Convert.ToBase64String(result);
        }

        [HttpPost]
        [Route("{userid}/{id}/noentropy")]
        public string PostNoEnctropy(
            [FromQuery] string userId,
            [FromQuery] string challengeId,
            [FromBody] string value)
        {
            SHA256 hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));
            var data = Convert.FromBase64String(value);
            var result = this.blockCipherOracleManager.EncryptOracle(data, seed, false);
            return Convert.ToBase64String(result);
        }
    }
}
