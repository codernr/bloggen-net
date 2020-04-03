using System.Collections.Generic;

namespace Bloggen.Net.Config
{
    public class SiteConfig
    {
        public string Title { get; set; } = string.Empty;

        public string SubHeading { get; set; } = string.Empty;

        public string Template { get; set; } = string.Empty;

        public int PostsPerPage { get; set; } = 10;

        public string Url { get; set; } = "/";

        public string DateFormat { get; set; } = "yyyy-MM-dd";

        public MetaProperty[] MetaProperties { get; set; } = new MetaProperty[]{};

        public string? GTag { get; set; }

        public string? DisqusId { get; set; }
    }

    public class MetaProperty
    {
        public string Property { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}