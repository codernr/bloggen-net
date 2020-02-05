using System.Collections.Generic;
using System.IO;

namespace Bloggen.Net.Source
{
    public interface ISourceHandler
    {
        Stream GetTemplate();

        IEnumerable<(string partialName, Stream stream)> GetLayouts();

        IEnumerable<(string partialName, Stream stream)> GetPartials();
    }
}