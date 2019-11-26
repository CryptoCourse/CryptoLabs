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

        private readonly static string debugFlag = "debug";

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
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + "GetEncryptedToken"));

                var plainText = Encoding.ASCII.GetBytes(TokenHelper.GetSecretTokenUser(seed));
                //#Q_
                if (challengeId == PaddingOracleController.debugFlag)
                {
                    plainText = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, };
                }

                var plainTextWithMac = this.paddingOracleManger.ApplyMac(
                    plainText,
                    seed);

                // change seed to make independant encryption key
                seed[0] ^= 255;
                var ciphertext = this.paddingOracleManger.EncryptCbc(
                    plainTextWithMac,
                    seed,
                    true);
                return HexHelper.HexFromByteArray(ciphertext);
            }
        }


        [HttpGet]
        [Route("{userId}/{challengeId}/ValidateEncryptedToken/{encryptedToken}")]
        public string ValidateEncryptedToken(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string encryptedToken)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + "GetEncryptedToken"));
                var encryptedTokenBytes = HexHelper.StringToByteArray(encryptedToken);

                // change seed to make independant encryption key
                seed[0] ^= 255;
                try
                {
                    byte[] plainTextWithMac;
                    try
                    {
                        plainTextWithMac = this.paddingOracleManger.DecryptCbc(encryptedTokenBytes, seed);
                    }
                    catch (Exception ex)
                    {
                        var debug = this.paddingOracleManger.DecryptCbc(encryptedTokenBytes, seed, PaddingMode.None);
                        throw;
                    }

                    // change it back
                    seed[0] ^= 255;
                    var plainText = this.paddingOracleManger.VerifyMac(
                        plainTextWithMac,
                        seed);
                    var stringToken = Encoding.ASCII.GetString(plainText);

                    if (TokenHelper.ValidateTokenString(stringToken) && TokenHelper.ValidateTokenUser(stringToken, seed))
                    {
                        return "Token have been decoded and validated";
                    }
                    return "Token is incorrect";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        [HttpGet]
        [Route("{userId}/{challengeId}/ValidateRawToken/{rawToken}")]
        public string ValidateRawToken(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromRoute] string rawToken)
        {
            using (var hash = SHA256.Create())
            {
                var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId + "GetEncryptedToken"));

                if (TokenHelper.ValidateTokenString(rawToken) && TokenHelper.ValidateTokenUser(rawToken, seed))
                {
                    return Convert.ToBase64String(Encoding.ASCII.GetBytes("Raw Token decoded and validated. Wellcome to secretNet!"));
                }
            }
            return Convert.ToBase64String(Encoding.ASCII.GetBytes("Token is incorrect"));
        }
    }
}
