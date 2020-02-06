using System.Collections.Generic;
using Bloggen.Net.Source;

namespace Bloggen.Net.Model
{
    public class Context : IContext
    {
        private readonly List<Post> posts = new List<Post>();

        private readonly List<Tag> tags = new List<Tag>();

        private readonly ISourceHandler sourceHandler;

        public IEnumerable<Post> Posts
        {
            get { return this.posts; }
        }

        public IEnumerable<Tag> Tags
        {
            get { return this.tags; }
        }

        public Context(ISourceHandler sourceHandler)
        {
            this.sourceHandler = sourceHandler;
        }
    }
}