using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Bloggen.Net.Config;
using Microsoft.Extensions.Options;

namespace Bloggen.Net.Source
{
    public class FileSystemSourceHandler : ISourceHandler
    {
        private const string TEMPLATES_DIRECTORY = "templates";

        private const string TEMPLATE_NAME = "index.hbs";

        private const string LAYOUTS_DIRECTORY = "layouts";

        private static readonly string[] LAYOUTS = {"page", "post", "archive", "tags"};

        private const string PARTIALS_DIRECTORY = "partials";
        
        private readonly IFileSystem fileSystem;

        private readonly string templatePath;

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
            return this.fileSystem.Directory.EnumerateFiles(
                this.fileSystem.Path.Combine(this.templatePath, PARTIALS_DIRECTORY))
                .Select(path => 
                    (this.fileSystem.Path.GetFileNameWithoutExtension(path), 
                    this.fileSystem.FileStream.Create(path, FileMode.Open)));
        }
    }
}