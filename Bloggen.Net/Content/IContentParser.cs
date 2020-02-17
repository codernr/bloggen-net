namespace Bloggen.Net.Content
{
    public interface IContentParser
    {
        string RenderPost(string fileName);

        string RenderPage(string fileName);
    }
}