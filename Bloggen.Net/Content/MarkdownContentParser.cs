using System.IO;
using System.Threading.Tasks;
using Bloggen.Net.Source;
using Markdig;

namespace Bloggen.Net.Content
{
    public class MarkdownContentParser : IContentParser
    {
        private readonly ISourceHandler sourceHandler;

        private readonly MarkdownPipeline pipeline;

        public MarkdownContentParser(ISourceHandler sourceHandler)
        {
            this.sourceHandler = sourceHandler;

            this.pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .Build();
        }

        public void RenderPost(string fileName, TextWriter writer)
        {
            Markdown.ToHtml(
                this.sourceHandler.GetPost(fileName),
                writer,
                this.pipeline);
        }
    }
}