using System.Collections.Generic;
using System.IO;

namespace Bloggen.Net.Source
{
    public interface ISourceHandler
    {
        Stream GetLayout();

        IEnumerable<Stream> GetPartials();
    }
}