using System.Collections.Generic;
using System.IO;

namespace Bloggen.Net.Source
{
    public interface ISourceHandler
    {
        string AssetsPath { get; }

        string TemplateAssetsPath { get; }

        Stream GetTemplate();

        IEnumerable<(string partialName, Stream stream)> GetLayouts();

        IEnumerable<(string partialName, Stream stream)> GetPartials();

        IEnumerable<(string fileName, Stream stream)> GetPosts();

        IEnumerable<(string fileName, Stream stream)> GetPages();

        string GetPost(string fileName);

        string GetPage(string fileName);
    }
}