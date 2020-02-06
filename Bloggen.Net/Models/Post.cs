using System.Collections.Generic;

namespace Bloggen.Net.Models
{
    public class Post
    {
        public string? Title { get; set; }

        public string? Excerpt { get; set; }

        public List<string>? Tags { get; set; }
    }
}