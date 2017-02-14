using System.IO;
using Newtonsoft.Json;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText("../../schema.json");
            var html = File.ReadAllText("../../source.html");
            var schema = ParserSchema.FromJson(json);
            var scrapper = new Parser(schema);
            var result = scrapper.Parse(html);
            var jsonOutput = JsonConvert.SerializeObject(result);
            File.WriteAllText("../../output.json", jsonOutput);
        }
    }
}
