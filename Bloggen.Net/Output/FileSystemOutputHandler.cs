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

        private readonly object site;

        public FileSystemOutputHandler(
            CommandLineOptions commandLineOptions,
            IFileSystem fileSystem,
            IContext<Post, Tag, Page> context,
            ITemplateHandler templateHandler,
            IContentParser contentParser,
            IOptions<SiteConfig> siteConfig,
            ISourceHandler sourceHandler)
        {
            (this.commandLineOptions, this.fileSystem, this.context, this.templateHandler, this.contentParser, this.siteConfig, this.sourceHandler) =
            (commandLineOptions, fileSystem, context, templateHandler, contentParser, siteConfig.Value, sourceHandler);

            this.site = new { config = this.siteConfig, tags = this.context.Tags, pages = this.context.Pages };
        }

        public void Generate()
        {
            this.ClearOutput();

            this.GenerateContextUrls();

            this.Render(this.context.Posts, "post", p => this.contentParser.RenderPost(p.FileName));

            this.Render(this.context.Tags, "tag");

            this.Render(this.context.Pages, "page", p => this.contentParser.RenderPage(p.FileName));

            this.RenderPostPages();

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
            
            this.CreateOutputSubDirectory(POSTS_DIRECTORY);
            this.CreateOutputSubDirectory(TAGS_DIRECTORY);
            this.CreateOutputSubDirectory(POST_PAGES_DIRECTORY);
        }

        private void CreateOutputSubDirectory(string directory)
        {
            this.fileSystem.Directory.CreateDirectory(
                this.fileSystem.Path.Combine(this.commandLineOptions.OutputDirectory, directory));
        }

        private void GenerateContextUrls()
        {
            foreach (var p in this.context.Posts)
            {
                p.Url = $"{POSTS_DIRECTORY}/{p.FileName}";
            }

            foreach (var t in this.context.Tags)
            {
                t.Url = $"{TAGS_DIRECTORY}/{t.Name.ToLower()}";
            }

            foreach (var p in this.context.Pages)
            {
                p.Url = $"{p.FileName}";
            }
        }

        private void Render<T>(IEnumerable<T> items, string layout, Func<T, string>? getContent = null) where T : class, IResource
        {
            foreach(var item in items)
            {
                var p = this.fileSystem.Path.Combine(this.commandLineOptions.OutputDirectory, item.Url);
                
                using var sw = this.fileSystem.File.CreateText($"{p}.{EXTENSION}");

                this.templateHandler.Write(sw, layout, item, this.site, getContent != null ? getContent(item) : null);
            }
        }

        private void RenderPostPages()
        {
            var list = this.GeneratePostPages();

            this.RenderPostPage(list[0], list.Count, this.commandLineOptions.OutputDirectory, $"index.{EXTENSION}");

            var node = list[0].Next;

            while (node != null)
            {
                this.RenderPostPage(node, list.Count, this.commandLineOptions.OutputDirectory, $"{node.Url}.{EXTENSION}");

                node = node.Next;
            }
        }

        private void GenerateTagsIndex()
        {
            using var sw = this.fileSystem.File.CreateText(
                this.fileSystem.Path.Combine(this.commandLineOptions.OutputDirectory, TAGS_DIRECTORY, $"index.{EXTENSION}"));

            this.templateHandler.Write(sw, "tags", this.context.Tags.OrderBy(t => t.Name), this.site);
        }

        private void RenderPostPage<T>(PaginationNode<T> node, int totalCount, params string[] pathParts) where T : class, IResource
        {
            node.TotalCount = totalCount;

            using var sw = this.fileSystem.File.CreateText(this.fileSystem.Path.Combine(pathParts));

            this.templateHandler.Write(sw, "list", node, this.site, null);
        }

        private List<PaginationNode<Post>> GeneratePostPages()
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
                    list[i].Url = $"{POST_PAGES_DIRECTORY}/{(list[i].PageNumber)}";
                    list[i].Previous = list[i - 1];
                    list[i - 1].Next = list[i];
                }
                else
                {
                    list[i].Url = string.Empty;
                }
            }

            return list;
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