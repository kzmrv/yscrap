using System;
using System.IO;
using NUnit.Framework;

namespace Parser.Tests
{
    [TestFixture]
    public class ParserFixture
    {
        [Test]
        public void When_parsing_some_simple_html()
        {
            var directory = Directory.GetCurrentDirectory();
            Console.WriteLine(directory);
            var json = File.ReadAllText("Tests\\simple_schema.json");
            var html = File.ReadAllText("Tests\\book_example.html");
            var schema = ParserSchema.FromJson(json);
            var parser = new Parser(schema);
            parser.Parse(html);
        }
    }
}
