using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bloggen.Net.Model;

namespace Bloggen.Net.Output.Implementation
{
    public class Post : IPost
    {
        public string FileName { get; set; } = null!;
        
        [Required]
        public string? Title { get; set; }

        public string? Excerpt { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public List<ITag> TagReferences { get; set; } = new List<ITag>();
    }
}