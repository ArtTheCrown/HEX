using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HEX.HEX
{
    public class JSONReader
    {
        public string? token { get; set; }
        public string? prefix { get; set; }
        public string? connectionString { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("ArtTheCrown/VanguardSettings.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure? data = JsonConvert.DeserializeObject<JSONStructure>(json);

                this.token = data?.token;
                this.prefix = data?.prefix;
                this.connectionString = data?.connectionString;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string? token { get; set; }
        public string? prefix { get; set; }
        public string? connectionString { get; set; }
    }
}
