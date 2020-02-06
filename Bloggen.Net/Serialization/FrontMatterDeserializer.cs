using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Bloggen.Net.Serialization
{
    public class FrontMatterDeserializer : IFrontMatterDeserializer
    {
        private readonly IDeserializer deserializer;

        private readonly Func<TextReader, IParser> parserFactory;

        public FrontMatterDeserializer(IDeserializer deserializer, Func<TextReader, IParser> parserFactory) =>
            (this.deserializer, this.parserFactory) = (deserializer, parserFactory);

        public T Deserialize<T>(TextReader input)
        {
            var parser = this.parserFactory(input);

            parser.Consume<StreamStart>();
            parser.Accept<DocumentStart>(out var evt);

            return this.deserializer.Deserialize<T>(parser);
        }
    }
}