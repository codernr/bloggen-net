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

        IEnumerable<(string fileName, Stream stream)> GetPages();

        string GetPost(string fileName);

        string GetPage(string fileName);
    }
}