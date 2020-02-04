using System.Collections.Generic;
using System.IO;

namespace Bloggen.Net.Source
{
    public interface ISourceHandler
    {
        TextReader GetLayout();

        IEnumerable<TextReader> GetPartials();
    }
}