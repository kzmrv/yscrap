using System.IO;
using Newtonsoft.Json;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText("../schema.json");
            var html = File.ReadAllText("../source.html");
            var schema = ParserSchema.FromJson(json);
            var parser = new Parser(schema);
            var result = parser.Parse(html);
            var jsonOutput = JsonConvert.SerializeObject(result);
            File.WriteAllText("../output.json", jsonOutput);
        }
    }
}
