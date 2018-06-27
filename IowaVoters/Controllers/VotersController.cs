/*
 * reddit.com/r/dailyprogrammer
 * [2017-09-29] Challenge #333 [Hard] Build a Web API-driven Data Site
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using IowaVoters.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IowaVoters.Controllers
{
    
    public class VotersController : ApiController
    {
        private VotersRepository _votersRepository;

        public VotersController(VotersRepository votersRepository)
        {
            this._votersRepository = votersRepository;
        }

        [Route("voters/get_voters_where")]
        public IHttpActionResult GetVotersWhere([FromUri] VotersRequest request)
        {
            request = request == null ? new VotersRequest() : request;
            var result = _votersRepository.GetVotersWhere(request);
            if (result != null)
            {
                var json = new JObject() as dynamic;
                json.result = new JArray(JArray.FromObject(result));
                json.start = request.Start + request.Limit;
                return Ok(json);
            }
            return NotFound();
        }
    }
}
