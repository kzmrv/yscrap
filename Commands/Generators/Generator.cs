using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Parser.Commands.Generators
{
    public abstract class Generator
    {
        static readonly Regex attributeGeneratorRegex = new Regex(
            @"(?<path>.*)@(?<attribute>[^\]]+)$", RegexOptions.Compiled);
        public static Generator Create(string pattern)
        {
            if (pattern.EndsWith(InnerTextGenerator.InnerTextGeneratorSignature))
            {
                return new InnerTextGenerator(pattern);
            }

            var attributeGeneratorMatch = attributeGeneratorRegex.Match(pattern);
            if (attributeGeneratorMatch.Success)
            {
                return new AttributeGenerator(attributeGeneratorMatch.Groups["path"].Value,
                    attributeGeneratorMatch.Groups["attribute"].Value);
            }

            throw new ArgumentException("Unknown generator");
        }

        public abstract string Generate(HtmlNode node);
    }
}
