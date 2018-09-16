using EasyPatch.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EasyPatch.WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

       [HttpPatch]
        public IHttpActionResult Post(TestPatchObject model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok();
        }
    }
}
