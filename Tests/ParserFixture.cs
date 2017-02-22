using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Parser.Tests
{
    [TestFixture]
    public class ParserFixture
    {
        static readonly string directory = TestContext.CurrentContext.TestDirectory;
        static string ReadFile(string path) => File.ReadAllText(Path.Combine(directory, path));

        static ExpandoObject Parse(string schemaPath, string sourcePath)
        {
            var schemaJson = ReadFile(Path.Combine("Tests//Data", schemaPath));
            var source = ReadFile(Path.Combine("Tests//Data", sourcePath));
            var schema = ParserSchema.FromJson(schemaJson);
            var parser = new Parser(schema);
            return parser.Parse(source);
        }

        [Test]
        public void When_parsing_some_simple_html()
        {
            var obj = Parse("simple_schema.json",
                "book_example.html")
                as IDictionary<string, object>;
            obj.Should().ContainKey("title");
            obj["title"].Should().BeOfType<ExpandoObject>();
            var title = (ExpandoObject)obj["title"];
            title.Should().ContainKey("name");
        }

        [Test]
        public void When_parsing_schema_with_attribute_generator()
        {
            dynamic obj = Parse("schema_with_attribute.json",
                "book_example.html");
            ((string)obj.title.name).Should().BeEquivalentTo("valueId");
        }

        [Test]
        public void When_parsing_simple_schema_with_array()
        {
            dynamic obj = Parse("schema_with_multi_selector.json", "book_store_example.html");
            dynamic books = obj.books;
            ((string) obj.name).Should().BeEquivalentTo("book store");
            ((string) books[0].id).Should().Be("0");
            ((string)books[1].id).Should().Be("1");
            ((string)books[2].id).Should().Be("2");
            ((string)obj.books[0].title).Should().BeEquivalentTo("War and peace");
            ((string)obj.books[1].title).Should().BeEquivalentTo("Alice in Wonderland");
            ((string)obj.books[2].title).Should().BeEquivalentTo("Harry Potter");
        }
    }
}
