using System.Collections.Generic;

namespace Bloggen.Net.Model
{
    public class Tag
    {
        public string? Name { get; set; }

        public List<Post> PostReferences { get; set; } = new List<Post>();
    }
}