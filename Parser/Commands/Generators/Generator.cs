using System;
using HtmlAgilityPack;

namespace Parser.Commands.Generators
{
    public abstract class Generator
    {
        public static Generator Create(string pattern)
        {
            if (pattern.EndsWith(InnerTextGenerator.InnerTextGeneratorSignature))
            {
                return new InnerTextGenerator(pattern);
            }
            throw new ArgumentException("Unknown generator");
        }

        public abstract string Generate(HtmlNode node);
    }
}
