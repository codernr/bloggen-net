using System.Collections.Generic;
using Bloggen.Net.Output.Implementation;

namespace Bloggen.Net.Output
{
    public class PaginationNode<T> : IResource where T : class
    {
        private readonly int pageNumber;

        private readonly IEnumerable<T> items;

        public int PageNumber => this.pageNumber;

        public IEnumerable<T> Items => this.items;

        public string? Url { get; set; }

        public PaginationNode(int pageNumber, IEnumerable<T> items) =>
            (this.pageNumber, this.items) = (pageNumber, items);
    }
}