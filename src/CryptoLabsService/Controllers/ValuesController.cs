namespace CryptoLabsService.Controllers
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/")]
    public class ValuesController : Controller
    {
        // GET api/
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "Api is working!" };
        }

        // GET api/5
        [HttpGet("values/{id}")]
        public string Get(string id)
        {
            return $"GET request for id = {id}";
        }

        // POST api/
        [HttpPost("values")]
        public string Post([FromBody] string value)
        {
            return $"POST request for value = {value}";
        }

        // PUT api/values/5
        [HttpPut("values/{id}")]
        public void Put(string id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("values/{id}")]
        public void Delete(int id)
        {
        }
    }
}