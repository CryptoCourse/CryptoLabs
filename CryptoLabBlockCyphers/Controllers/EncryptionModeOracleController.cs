using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoLabBlockCiphers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoLabBlockCyphers.Controllers
{
    [Route("api/[controller]")]
    internal class EncryptionModeOracleController : Controller
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

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            this.blockCipherOracleManager.EncryptCbc(null, null);
        }
    }
}
