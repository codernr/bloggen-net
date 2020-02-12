using System.Collections.Generic;

namespace Bloggen.Net.Model
{
    public interface IContext<TPost, TTag> where TPost : IPost where TTag : ITag
    {
        IEnumerable<TPost> Posts { get; }

        IEnumerable<TTag> Tags { get; }
    }
}