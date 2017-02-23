using System.Dynamic;
using Newtonsoft.Json;

namespace Parser
{
    public class ParserSchema
    {
        public ScrapperBlock[] children;

        public static ParserSchema FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ParserSchema>(json);
        }
    }

    public class ScrapperBlock
    {
        public string select;
        public string value;
        public ExpandoObject exactValue;
        public string create;
        public string createArray;
        public ScrapperBlock[] children;
    }
}
