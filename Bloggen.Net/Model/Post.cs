using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bloggen.Net.Model
{
    public class Post
    {
        public string FileName { get; set; } = null!;
        
        [Required]
        public string? Title { get; set; }

        public string? Excerpt { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public List<Tag> TagReferences { get; set; } = new List<Tag>();
    }
}