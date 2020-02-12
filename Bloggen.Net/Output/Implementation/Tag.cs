using System.Collections.Generic;
using Bloggen.Net.Model;

namespace Bloggen.Net.Output.Implementation
{
    public class Tag : ITag
    {
        public string? Name { get; set; }

        public List<IPost> PostReferences { get; set; } = new List<IPost>();

        public string Url
        {
            get { return $"/{FileSystemOutputHandler.TAGS_DIRECTORY}/{this.Name}"; }
        }
    }
}