using System.Collections.Generic;

namespace Bloggen.Net.Model
{
    public interface IContext<TPost, TTag, TPage> where TPost : IPost where TTag : ITag where TPage : IPage
    {
        IEnumerable<TPost> Posts { get; }

        IEnumerable<TTag> Tags { get; }

        IEnumerable<TPage> Pages { get; }
    }
}