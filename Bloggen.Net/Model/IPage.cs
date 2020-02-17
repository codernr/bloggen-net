namespace Bloggen.Net.Model
{
    public interface IPage
    {
        string FileName { get; set; }
        
        string? Title { get; set; }
    }
}