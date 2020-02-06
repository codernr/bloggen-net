using System.Collections.Generic;

namespace Bloggen.Net.Models
{
    public class Post
    {
        public string FileName { get; set; } = null!;
        
        public string? Title { get; set; }

        public string? Excerpt { get; set; }

        public List<string>? Tags { get; set; }

        public List<Tag> TagReferences { get; set; } = new List<Tag>();
    }
}