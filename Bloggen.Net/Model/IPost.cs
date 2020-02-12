using System.Collections.Generic;

namespace Bloggen.Net.Model
{
    public interface IPost
    {
        string FileName { get; set; }

        string? Title { get; set; }

        string? Excerpt { get; set; }

        List<string> Tags { get; set; }

        List<ITag> TagReferences { get; set; }
    }
}