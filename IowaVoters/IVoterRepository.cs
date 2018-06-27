using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using IowaVoters.Models;

namespace IowaVoters
{
    public interface IVoterRepository
    {
        IEnumerable<Dictionary<string, JToken>> GetVotersWhere(VotersRequest request);
    }
}
