namespace CryptoLabsService.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using CryptoLabsService.Managers;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/StreamIntegrity")]
    public class StreamIntegrityController : Controller
    {

        private readonly string stringData = "Here is some data to encrypt";

        private readonly string secretToken = "Token: 8ce08ad2d48d7d356db43";

        private readonly StreamCipherIntegrityManager streamCipherIntegrityManager;

        public StreamIntegrityController(StreamCipherIntegrityManager streamCipherIntegrityManager)
        {
            this.streamCipherIntegrityManager = streamCipherIntegrityManager;
        }

        // GET api/
        [HttpGet]
        public string Get()
        {
            return "operating";
        }

        [HttpGet]
        [Route("{userId}/{challengeId}")]
        public string Get(
            [FromRoute] string userId,
            [FromRoute] string challengeId)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var ciphertext = this.streamCipherIntegrityManager.EncryptCrt(
                Encoding.ASCII.GetBytes(this.stringData),
                seed,
                true);
            return Convert.ToBase64String(ciphertext);
        }


        [HttpGet]
        [Route("{userId}/{challengeId}/noentropy")]
        public string GetNoEntropy(
            [FromRoute] string userId,
            [FromRoute] string challengeId)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var ciphertext = this.streamCipherIntegrityManager.EncryptCrt(
                Encoding.ASCII.GetBytes(this.stringData),
                seed,
                false);
            return Convert.ToBase64String(ciphertext);
        }

        [HttpPost]
        [Route("{userId}/{challengeId}")]
        public string Post(
            [FromRoute] string userId,
            [FromRoute] string challengeId,
            [FromBody] string value)
        {
            var hash = SHA256.Create();
            var seed = hash.ComputeHash(Encoding.ASCII.GetBytes(userId + challengeId));

            var ciphertext = Convert.FromBase64String(value);
            if (ciphertext.Length < 16)
            {
                throw new Exception("invalid ct len");
            }

            var plaintext = this.streamCipherIntegrityManager.DecryptCtr(
                Encoding.ASCII.GetBytes(this.stringData),
                seed);

            var secretHash = hash.ComputeHash(Encoding.ASCII.GetBytes(this.secretToken));
            var paintextHash = hash.ComputeHash(plaintext);

            //comparing hashes of inputed value, no need in const time cmp here
            if (paintextHash.SequenceEqual(secretHash))
            {
                return "Wellcome to secretNet!";
            }

            return $"Your decryptedData is [{plaintext}]";
        }
    }
}
