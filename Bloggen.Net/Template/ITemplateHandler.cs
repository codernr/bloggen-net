using System.IO;

namespace Bloggen.Net.Template
{
    public interface ITemplateHandler
    {
        void Write(TextWriter writer, string layout, object data, string? content = null);
    }
}