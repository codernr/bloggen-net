using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Bloggen.Net.Config;
using Bloggen.Net.Content;
using Bloggen.Net.Model;
using Bloggen.Net.Output.Implementation;
using Bloggen.Net.Template;
using Microsoft.Extensions.Options;

namespace Bloggen.Net.Output
{
    public class FileSystemOutputHandler : IOutputHandler
    {
        public const string POSTS_DIRECTORY = "posts";

        public const string TAGS_DIRECTORY = "tags";

        public const string POST_PAGES_DIRECTORY = "pages";

        private const string EXTENSION = "html";

        private readonly string outputDirectory;

        private readonly IFileSystem fileSystem;

        private readonly IContext<Post, Tag, Page> context;

        private readonly ITemplateHandler templateHandler;

        private readonly IContentParser contentParser;

        private readonly SiteConfig siteConfig;

        public FileSystemOutputHandler(
            CommandLineOptions commandLineOptions,
            IFileSystem fileSystem,
            IContext<Post, Tag, Page> context,
            ITemplateHandler templateHandler,
            IContentParser contentParser,
            IOptions<SiteConfig> siteConfig) =>
            (this.outputDirectory, this.fileSystem, this.context, this.templateHandler, this.contentParser, this.siteConfig) =
            (commandLineOptions.OutputDirectory, fileSystem, context, templateHandler, contentParser, siteConfig.Value);

        public void Generate()
        {
            this.ClearOutput();

            this.Generate(POSTS_DIRECTORY, this.context.Posts, p => p.FileName, "post", p => this.contentParser.RenderPost(p.FileName));

            this.Generate(TAGS_DIRECTORY, this.context.Tags, t => t.Name, "tag");

            this.Generate(this.outputDirectory, this.context.Pages, p => p.FileName, "page", p => this.contentParser.RenderPage(p.FileName));
        }

        private void ClearOutput()
        {
            if (this.fileSystem.Directory.Exists(this.outputDirectory))
            {
                this.fileSystem.Directory.Delete(this.outputDirectory, true);
            }

            this.fileSystem.Directory.CreateDirectory(this.outputDirectory);
        }

        private void Generate<T>(
            string directory,
            IEnumerable<T> items,
            Func<T, string> nameSelector,
            string layout, Func<T, string>? getContent = null) where T : class
        {
            var path = this.fileSystem.Path.Combine(this.outputDirectory, directory);

            this.fileSystem.Directory.CreateDirectory(path);

            foreach(var item in items)
            {
                using var sw = this.fileSystem.File.CreateText(
                    $"{this.fileSystem.Path.Combine(path, nameSelector(item))}.{EXTENSION}"
                );

                this.templateHandler.Write(sw, layout, item, getContent != null ? getContent(item) : null);
            }
        }

        private void GeneratePostPages()
        {
            var list = this.CreatePostPages();

            // first page
            var firstSw = this.fileSystem.File.CreateText(
                this.fileSystem.Path.Combine(this.outputDirectory, $"index.{EXTENSION}"));

            this.templateHandler.Write(firstSw, "list", list.First!, null);

            // other pages
            var node = list.First?.Next;

            while (node != null)
            {
                var sw = this.fileSystem.File.CreateText(
                    this.fileSystem.Path.Combine(this.outputDirectory, POST_PAGES_DIRECTORY, $"{node.Value.PageNumber}.{EXTENSION}"));
                
                this.templateHandler.Write(sw, "list", node, null);

                node = node.Next;
            }
        }

        private LinkedList<PaginationNode<Post>> CreatePostPages()
        {
            var postCount = this.context.Posts.Count();

            var list = new LinkedList<PaginationNode<Post>>();

            for (int i = 0; i * this.siteConfig.PostsPerPage < postCount; i++)
            {
                list.AddLast(new PaginationNode<Post>(
                    i + 1,
                    this.context.Posts.Skip(i * this.siteConfig.PostsPerPage).Take(this.siteConfig.PostsPerPage)));
            }

            return list;
        }
    }
}