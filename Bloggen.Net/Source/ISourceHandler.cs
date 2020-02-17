using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bloggen.Net.Source
{
    public interface ISourceHandler
    {
        Stream GetTemplate();

        IEnumerable<(string partialName, Stream stream)> GetLayouts();

        IEnumerable<(string partialName, Stream stream)> GetPartials();

        IEnumerable<(string fileName, Stream stream)> GetPosts();

        string GetPost(string fileName);
    }
}