using IowaVoters.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IowaVoters
{
    public class VotersRepository : IVoterRepository
    {
        private static string _data = System.AppDomain.CurrentDomain.BaseDirectory + "rows.json";

        public VotersRepository()
        {
            this.Columns = new List<string>();

            using (var reader = new StreamReader(_data))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    do
                    {
                        jsonReader.Read();
                        if (jsonReader.Value == null || !jsonReader.Value.Equals("columns"))
                        {
                            continue;
                        }                        
                        jsonReader.Read();
                        var jsonSerializer = new JsonSerializer();
                        JArray columns = jsonSerializer.Deserialize<JArray>(jsonReader);
                        foreach (var column in columns.Children().Take(24))
                        {
                            this.Columns.Add(column["name"].ToObject<string>());
                        }                    
                    } while (Columns.Count == 0);
                }
            }
            
        }
        public IEnumerable<Dictionary<string, JToken>> GetVotersWhere(VotersRequest request)
        {
            var voters = new List<Dictionary<string, JToken>>();
            using (var r = new StreamReader(_data))
            {
                using (var reader = new JsonTextReader(r))
                {
                    do
                    {
                        reader.Read();
                        if ( reader.Value == null || !reader.Value.Equals("data"))
                        {
                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                reader.Skip();
                            }
                            continue;
                        }
                        reader.Read();
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                        {
                            JsonSerializer s = new JsonSerializer();
                            JArray row = s.Deserialize<JArray>(reader);
                            var party = request.Party == VotersRequest.PartyEnum.Any ? "" : "|" + request.Party.ToString();
                            party += (request.Status == VotersRequest.StatusEnum.Any ? "" : " - " + request.Status.ToString());
                            var allowedColumns = "County|Date|Grand Total" + party;
                            var result = new Dictionary<string, JToken>();
                            int currentColumn = 0;
                            foreach (var column in row.Take(24).Values())
                            {
                                if (Regex.IsMatch(Columns[currentColumn], allowedColumns))
                                {
                                    result.Add(Columns[currentColumn], column);
                                }
                                currentColumn++;
                            }                     
                            if ((string.IsNullOrEmpty(request.County) || request.County == ((JValue)result["County"]).ToObject<string>().ToLower()) &&
                                (request.Month == 0 || request.Month == ((JValue)result["Date"]).ToObject<DateTime>().Month))
                            {
                                voters.Add(result);
                            }
                        }
                    } while(reader.TokenType != JsonToken.None);

                }
            }
            return voters.Count > 0 ? new List<Dictionary<string, JToken>>(voters.Skip(request.Start).Take(request.Limit)) : null;
        }

        public IList<string> Columns { get; private set; }

    }
}