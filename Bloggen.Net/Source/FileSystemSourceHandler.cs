using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Bloggen.Net.Source
{
    public class FileSystemSourceHandler : ISourceHandler
    {
        private const string TEMPLATE_NAME = "index.hbs";

        private const string LAYOUTS_DIRECTORY = "layouts";

        private static readonly string[] LAYOUTS = {"page", "post", "archive", "tags"};

        private const string PARTIALS_DIRECTORY = "partials";
        
        private readonly IFileSystem fileSystem;

        private readonly CommandLineOptions commandLineOptions;

        public FileSystemSourceHandler(IFileSystem fileSystem, CommandLineOptions commandLineOptions) =>
            (this.fileSystem, this.commandLineOptions) = (fileSystem, commandLineOptions);

        public Stream GetTemplate()
        {
            return this.fileSystem.FileStream.Create(
                this.fileSystem.Path.Combine(this.commandLineOptions.SourceDirectory, TEMPLATE_NAME),
                FileMode.Open
            );
        }
        public IEnumerable<(string partialName, Stream stream)> GetLayouts()
        {
            var layoutsPath = this.fileSystem.Path.Combine(
                this.commandLineOptions.SourceDirectory, LAYOUTS_DIRECTORY);

            return LAYOUTS.Select(l => 
                (l, this.fileSystem.FileStream.Create(
                    this.fileSystem.Path.Combine(layoutsPath, $"{l}.hbs"), FileMode.Open)));
        }

        public IEnumerable<(string partialName, Stream stream)> GetPartials()
        {
            return this.fileSystem.Directory.EnumerateFiles(
                this.fileSystem.Path.Combine(this.commandLineOptions.SourceDirectory, PARTIALS_DIRECTORY))
                .Select(path => 
                    (this.fileSystem.Path.GetFileNameWithoutExtension(path), 
                    this.fileSystem.FileStream.Create(path, FileMode.Open)));
        }
    }
}