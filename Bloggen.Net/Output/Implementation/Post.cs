using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bloggen.Net.Config;
using Bloggen.Net.Model;

namespace Bloggen.Net.Output.Implementation
{
    public class Post : IPost, IResource
    {
        public string FileName { get; set; } = null!;
        
        [Required]
        public string? Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string? PostedBy { get; set; }

        public string? Excerpt { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public List<ITag> TagReferences { get; set; } = new List<ITag>();

        public string? Url { get; set; }

        public List<MetaProperty> MetaProperties = new List<MetaProperty>();
    }
}