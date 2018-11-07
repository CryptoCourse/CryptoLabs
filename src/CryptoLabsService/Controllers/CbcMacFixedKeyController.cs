namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/CbcMacFixedKey")]
    public class CbcMacFixedKeyController : Controller
    {
        private const int AesBlockSize = 16;

        private readonly CbcMacManager cbcMacManager;

        public CbcMacFixedKeyController(CbcMacManager cbcMacManager)
        {
            this.cbcMacManager = cbcMacManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpPost]
        [Route("{userId}/{challengeId}/{mac}")]
        public string PostData(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromBody] string data,
            [FromRoute] string mac)
        {

            var hash = SHA256.Create();

            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var decodedData = Convert.FromBase64String(data);
            var decodedMac = this.StringToByteArray(mac);

            var targetMac = this.cbcMacManager.ConputeMacWithKey(decodedData, seed)
                .Skip(AesBlockSize / 2)
                .ToArray();

            if (this.CompareArrays(targetMac, decodedMac))
            {
                return Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $"{Encoding.ASCII.GetString(decodedData)} have valid mac = {BitConverter.ToString(decodedMac).Replace("-", "")}"));
            }
            else
            {
                return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Encoding.ASCII.GetString(decodedData)} have invalid mac"));
            }
        }

        //[HttpPost]
        //[Route("{userId}/{challengeId}/Collide")]
        //public string PostTestCollizion(
        //    [FromRoute] string userId,
        //    [FromRoute] string challengeId,
        //    [FromBody] string data)
        //{

        //    var hash = SHA256.Create();

        //    var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

        //    JsonObject

        //    var decodedData1 = Convert.FromBase64String(data1);
        //    var decodedData2 = Convert.FromBase64String(data2);

        //    var targetMac1 = this.cbcMacManager.ConputeMacWithKey(decodedData1, seed);
        //    var targetMac2 = this.cbcMacManager.ConputeMacWithKey(decodedData2, seed);

        //    if (this.CompareArrays(targetMac1, targetMac2))
        //    {
        //        return Convert.ToBase64String(
        //            Encoding.ASCII.GetBytes(
        //                $"MACs are the same!"));
        //    }
        //    else
        //    {
        //        return Convert.ToBase64String(Encoding.ASCII.GetBytes($"MACs are different!"));
        //    }
        //}

        [HttpGet]
        [Route("{userId}/{challengeId}/Key")]
        public string GetKey([FromRoute] string userId, [FromRoute] string challengeId)
        {
            var hash = SHA256.Create();

            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            return Convert.ToBase64String(this.cbcMacManager.GetKey(seed));
        }

        private bool CompareArrays(byte[] first, byte[] second)
        {
            byte result = 0;
            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                result |= (byte)(first[i] ^ second[i]);
            }

            return result == 0;
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