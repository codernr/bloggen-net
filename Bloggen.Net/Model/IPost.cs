using System;
using System.Collections.Generic;

namespace Bloggen.Net.Model
{
    public interface IPost
    {
        string FileName { get; set; }

        DateTime CreatedAt { get; set; }

        string? Title { get; set; }

        string? Excerpt { get; set; }

        string? PostedBy { get; set; } 

        List<string> Tags { get; set; }

        List<ITag> TagReferences { get; set; }
    }
}