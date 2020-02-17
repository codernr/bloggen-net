using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Bloggen.Net.Config;
using Microsoft.Extensions.Options;

namespace Bloggen.Net.Source
{
    public class FileSystemSourceHandler : ISourceHandler
    {
        private const string TEMPLATES_DIRECTORY = "templates";

        private const string TEMPLATE_NAME = "index.hbs";

        private const string LAYOUTS_DIRECTORY = "layouts";

        private static readonly string[] LAYOUTS = {"page", "post", "archive", "tags", "tag"};

        private const string PARTIALS_DIRECTORY = "partials";

        private const string POSTS_DIRECTORY = "posts";
        
        private readonly IFileSystem fileSystem;

        private readonly string templatePath;

        private readonly string postsPath;

        public FileSystemSourceHandler(
            IFileSystem fileSystem,
            CommandLineOptions commandLineOptions,
            IOptions<SiteConfig> siteConfig)
        {
            this.fileSystem = fileSystem;

            this.templatePath = this.fileSystem.Path.Combine(
                commandLineOptions.SourceDirectory,
                TEMPLATES_DIRECTORY,
                siteConfig.Value.Template
            );

            this.postsPath = this.fileSystem.Path.Combine(
                commandLineOptions.SourceDirectory,
                POSTS_DIRECTORY
            );
        }

        public Stream GetTemplate()
        {
            return this.fileSystem.FileStream.Create(
                this.fileSystem.Path.Combine(this.templatePath, TEMPLATE_NAME),
                FileMode.Open);
        }

        public IEnumerable<(string partialName, Stream stream)> GetLayouts()
        {
            var layoutsPath = this.fileSystem.Path.Combine(this.templatePath, LAYOUTS_DIRECTORY);

            var files = LAYOUTS.Select(l => this.fileSystem.Path.Combine(layoutsPath, $"{l}.hbs"));

            foreach (var f in files)
            {
                 if (!this.fileSystem.File.Exists(f))
                 {
                     throw new FileNotFoundException("Layout file not found", fileName: f);
                 }
            }

            return LAYOUTS.Select(l => 
                (l, this.fileSystem.FileStream.Create(
                    this.fileSystem.Path.Combine(layoutsPath, $"{l}.hbs"), FileMode.Open)));
        }

        public IEnumerable<(string partialName, Stream stream)> GetPartials()
        {
            return this.EnumerateFiles(
                this.fileSystem.Path.Combine(this.templatePath, PARTIALS_DIRECTORY));
        }

        public IEnumerable<(string fileName, Stream stream)> GetPosts()
        {
            return this.EnumerateFiles(this.postsPath);
        }

        public string GetPost(string fileName)
        {
            var filePath = this.fileSystem.Directory.GetFiles(this.postsPath).First(f =>
                this.fileSystem.Path.GetFileNameWithoutExtension(f) == fileName);

            return this.fileSystem.File.ReadAllText(filePath);
        }

        private IEnumerable<(string name, Stream stream)> EnumerateFiles(string path)
        {
            return this.fileSystem.Directory.EnumerateFiles(path)
                .Select(p => 
                    (this.fileSystem.Path.GetFileNameWithoutExtension(p), 
                    this.fileSystem.FileStream.Create(p, FileMode.Open)));
        }
    }
}