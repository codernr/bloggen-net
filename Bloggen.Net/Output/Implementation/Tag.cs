using System.Collections.Generic;
using Bloggen.Net.Model;

namespace Bloggen.Net.Output.Implementation
{
    public class Tag : ITag, IResource
    {
        public string Name { get; set; } = null!;

        public List<IPost> PostReferences { get; set; } = new List<IPost>();

        public string? Url { get; set; }
    }
}