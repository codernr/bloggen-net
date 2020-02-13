using System.Collections.Generic;

namespace Bloggen.Net.Model
{
    public interface ITag
    {
        string Name { get; set; }

        List<IPost> PostReferences { get; set; }
    }
}