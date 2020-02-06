using System.Collections.Generic;

namespace Bloggen.Net.Model
{
    public interface IContext
    {
        IEnumerable<Post> Posts { get; }

        IEnumerable<Tag> Tags { get; }
    }
}