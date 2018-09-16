using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyPatch.Common.Interface;

namespace EasyPatch.CoreApi.Controllers
{
    public class Test: IPatchState
    {
        public string Value { get; set; }

        public void AddBoundProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, string>> Validate()
        {
            throw new NotImplementedException();
        }
    }
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post(Test value)
        {
            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
