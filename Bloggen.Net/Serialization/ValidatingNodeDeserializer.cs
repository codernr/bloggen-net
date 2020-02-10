using System;
using System.ComponentModel.DataAnnotations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Bloggen.Net.Serialization
{
    public class ValidatingNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer;

        public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer) =>
            this.nodeDeserializer = nodeDeserializer;

        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (this.nodeDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value))
            {
                var context = new ValidationContext(value);
                Validator.ValidateObject(value, context, true);
                return true;
            }

            return false;
        }
    }
}