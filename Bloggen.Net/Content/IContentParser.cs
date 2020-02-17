using System.IO;
using System.Threading.Tasks;

namespace Bloggen.Net.Content
{
    public interface IContentParser
    {
        void RenderPost(string fileName, TextWriter writer);
    }
}