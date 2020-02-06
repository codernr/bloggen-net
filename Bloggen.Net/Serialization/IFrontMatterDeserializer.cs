using System.IO;

namespace Bloggen.Net.Serialization
{
    public interface IFrontMatterDeserializer
    {
        T Deserialize<T>(TextReader input);
    }
}