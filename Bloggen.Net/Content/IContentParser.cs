using System.IO;

namespace Bloggen.Net.Content
{
    public interface IContentParser
    {
        void RenderPost(string fileName, TextWriter writer);
    }
}