namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using CryptoLabsService.Helpers;
    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/PaddingOracle")]
    public class PaddingOracleController : Controller
    {
        private const int AesBlockSize = 16;

        private const int BlockCount = 4;

        const string AvailableTokenChars = "0123456789abcdef";

        private readonly PaddingOracleManger paddingOracleManger;

        public PaddingOracleController(PaddingOracleManger paddingOracleManger)
        {
            this.paddingOracleManger = paddingOracleManger;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/GetEncryptedToken")]
        public string GetToken(
            [FromRoute] string userId,
            [FromRoute] string challengeId)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + "GetEncryptedToken"));

            var plainText = Encoding.ASCII.GetBytes(TokenHelper.GetSecretTokenUser(seed));

            var plainTextWithMac = this.paddingOracleManger.ApplyMac(
                plainText,
                seed);

            // change seed to make independant encryption key
            seed[0] ^= 255;
            var ciphertext = this.paddingOracleManger.EncryptCbc(
                plainTextWithMac,
                seed,
                true);
            return Convert.ToBase64String(ciphertext);
        }


        [HttpGet]
        [Route("{userId}/{challengeId}/ValidateEncryptedToken/{encryptedToken}")]
        public string ValidateEncryptedToken(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string encryptedToken)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + "GetEncryptedToken"));
            var encryptedTokenBytes = HexHelper.StringToByteArray(encryptedToken);

            // change seed to make independant encryption key
            seed[0] ^= 255;
            try
            {
                var plainTextWithMac = this.paddingOracleManger.DecryptCbc(encryptedTokenBytes, seed);

                // change it back
                seed[0] ^= 255;
                var plainText = this.paddingOracleManger.VerifyMac(
                    plainTextWithMac,
                    seed);
                var stringToken = Encoding.ASCII.GetString(plainText);

                if (TokenHelper.ValidateTokenString(stringToken) && TokenHelper.ValidateTokenUser(stringToken, seed))
                {
                    return Convert.ToBase64String(Encoding.ASCII.GetBytes("Token decoded and validated"));
                }
                return Convert.ToBase64String(Encoding.ASCII.GetBytes("Token is incorrect"));
            }
            catch (Exception ex)
            {
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(ex.ToString()));
            }
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/ValidateRawToken/{rawToken}")]
        public string ValidateRawToken(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string rawToken)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + "GetEncryptedToken"));
            
            if (TokenHelper.ValidateTokenString(rawToken) && TokenHelper.ValidateTokenUser(rawToken, seed))
            {
                return Convert.ToBase64String(Encoding.ASCII.GetBytes("Raw Token decoded and validated"));
            }
            return Convert.ToBase64String(Encoding.ASCII.GetBytes("Token is incorrect"));
        }
    }
}
