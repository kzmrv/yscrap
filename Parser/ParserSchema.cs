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
        public string create;
        public ScrapperBlock[] children;
    }
}
