using System.Collections.Generic;

namespace Bloggen.Net.Models
{
    public class Tag
    {
        public string? Name { get; set; }

        public List<Post> PostReferences { get; set; } = new List<Post>();
    }
}