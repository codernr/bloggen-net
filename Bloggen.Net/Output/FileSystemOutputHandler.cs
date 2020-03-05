using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Bloggen.Net.Config;
using Bloggen.Net.Content;
using Bloggen.Net.Model;
using Bloggen.Net.Output.Implementation;
using Bloggen.Net.Source;
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

        private readonly CommandLineOptions commandLineOptions;

        private readonly IFileSystem fileSystem;

        private readonly IContext<Post, Tag, Page> context;

        private readonly ITemplateHandler templateHandler;

        private readonly IContentParser contentParser;

        private readonly SiteConfig siteConfig;

        private readonly ISourceHandler sourceHandler;

        public FileSystemOutputHandler(
            CommandLineOptions commandLineOptions,
            IFileSystem fileSystem,
            IContext<Post, Tag, Page> context,
            ITemplateHandler templateHandler,
            IContentParser contentParser,
            IOptions<SiteConfig> siteConfig,
            ISourceHandler sourceHandler) =>
            (this.commandLineOptions, this.fileSystem, this.context, this.templateHandler, this.contentParser, this.siteConfig, this.sourceHandler) =
            (commandLineOptions, fileSystem, context, templateHandler, contentParser, siteConfig.Value, sourceHandler);

        public void Generate()
        {
            this.ClearOutput();

            this.Generate(POSTS_DIRECTORY, this.context.Posts, p => p.FileName, "post", p => this.contentParser.RenderPost(p.FileName));

            this.Generate(TAGS_DIRECTORY, this.context.Tags, t => t.Name, "tag");

            this.Generate(this.commandLineOptions.OutputDirectory, this.context.Pages, p => p.FileName, "page", p => this.contentParser.RenderPage(p.FileName));

            this.GeneratePostPages();

            this.GenerateTagsIndex();

            this.CopyAssets();
        }

        private void ClearOutput()
        {
            if (this.fileSystem.Directory.Exists(this.commandLineOptions.OutputDirectory))
            {
                this.fileSystem.Directory.Delete(this.commandLineOptions.OutputDirectory, true);
            }

            this.fileSystem.Directory.CreateDirectory(this.commandLineOptions.OutputDirectory);
        }

        private void Generate<T>(
            string directory,
            IEnumerable<T> items,
            Func<T, string> nameSelector,
            string layout, Func<T, string>? getContent = null) where T : class, IResource
        {
            var path = this.fileSystem.Path.Combine(this.commandLineOptions.OutputDirectory, directory);

            this.fileSystem.Directory.CreateDirectory(path);

            foreach(var item in items)
            {
                item.Url = this.GetUrl(directory, nameSelector(item));

                using var sw = this.fileSystem.File.CreateText(
                    $"{this.fileSystem.Path.Combine(path, nameSelector(item))}.{EXTENSION}"
                );

                this.templateHandler.Write(sw, layout, item, getContent != null ? getContent(item) : null);
            }
        }

        private void GeneratePostPages()
        {
            var list = this.CreatePostPages();

            this.GeneratePostPage(list[0], list.Count, this.GetUrl(), this.commandLineOptions.OutputDirectory, $"index.{EXTENSION}");

            var node = list[0].Next;

            while (node != null)
            {
                this.GeneratePostPage(
                    node, list.Count, this.GetUrl(POSTS_DIRECTORY, node.PageNumber.ToString()),
                    this.commandLineOptions.OutputDirectory, POST_PAGES_DIRECTORY, $"{node.PageNumber}.{EXTENSION}");

                node = node.Next;
            }
        }

        private void GenerateTagsIndex()
        {
            using var sw = this.fileSystem.File.CreateText(
                this.fileSystem.Path.Combine(this.commandLineOptions.OutputDirectory, TAGS_DIRECTORY, $"index.{EXTENSION}"));

            this.templateHandler.Write(sw, "tags", this.context.Tags.OrderBy(t => t.Name));
        }

        private void GeneratePostPage<T>(PaginationNode<T> node, int totalCount, string url, params string[] pathParts) where T : class, IResource
        {
            node.Url = url;
            node.TotalCount = totalCount;

            using var sw = this.fileSystem.File.CreateText(this.fileSystem.Path.Combine(pathParts));

            this.templateHandler.Write(sw, "list", node, null);
        }

        private List<PaginationNode<Post>> CreatePostPages()
        {
            var postCount = this.context.Posts.Count();

            var list = new List<PaginationNode<Post>>();

            var ordered = this.context.Posts.OrderByDescending(p => p.CreatedAt);

            for (int i = 0; i * this.siteConfig.PostsPerPage < postCount; i++)
            {
                list.Add(new PaginationNode<Post>(
                    i + 1,
                    ordered.Skip(i * this.siteConfig.PostsPerPage).Take(this.siteConfig.PostsPerPage)));
                
                if (i > 0)
                {
                    list[i].Previous = list[i - 1];
                    list[i - 1].Next = list[i];
                }
            }

            return list;
        }

        private string GetUrl(params string[] pathParts)
        {
            var builder = new UriBuilder(this.siteConfig.Url);

            builder.Path = string.Join('/', pathParts);

            return builder.Uri.AbsoluteUri;
        }

        private void CopyAssets()
        {
            var destination = this.fileSystem.Path.Combine(this.commandLineOptions.OutputDirectory, "assets");

            this.CopyDirectory(this.sourceHandler.AssetsPath, destination);
            this.CopyDirectory(this.sourceHandler.TemplateAssetsPath, destination);
        }

        private void CopyDirectory(string source, string destination)
        {
            var sourceDirectory = this.fileSystem.DirectoryInfo.FromDirectoryName(source);

            if (!sourceDirectory.Exists)
            {
                return;
            }

            if (!this.fileSystem.DirectoryInfo.FromDirectoryName(destination).Exists)
            {
                this.fileSystem.Directory.CreateDirectory(destination);
            }

            foreach (var f in sourceDirectory.GetFiles())
            {
                f.CopyTo(this.fileSystem.Path.Combine(destination, f.Name), true);
            }

            foreach (var d in sourceDirectory.GetDirectories())
            {
                this.CopyDirectory(d.FullName, this.fileSystem.Path.Combine(destination, d.Name));
            }
        }
    }
}