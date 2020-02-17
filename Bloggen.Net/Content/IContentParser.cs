using System.IO;
using System.Threading.Tasks;

namespace Bloggen.Net.Content
{
    public interface IContentParser
    {
        Task RenderPostAsync(string fileName, TextWriter writer);
    }
}