namespace Bloggen.Net.Config
{
    public class SiteConfig
    {
        public string Title { get; set; } = string.Empty;

        public string Template { get; set; } = string.Empty;

        public int PostsPerPage { get; set; } = 10;

        public string Url { get; set; } = "/";
    }
}