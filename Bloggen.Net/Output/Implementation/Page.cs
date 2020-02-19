using System.ComponentModel.DataAnnotations;
using Bloggen.Net.Model;

namespace Bloggen.Net.Output.Implementation
{
    public class Page : IPage, IResource
    {
        public string FileName { get; set; } = null!;

        [Required]
        public string? Title { get; set; }

        public string? Url { get; set; }
    }
}