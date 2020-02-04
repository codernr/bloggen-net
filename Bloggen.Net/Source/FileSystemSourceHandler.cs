using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Bloggen.Net.Source
{
    public class FileSystemSourceHandler : ISourceHandler
    {
        private const string LAYOUT_NAME = "layout.hbs";

        private const string PARIALS_DIRECTORY = "partials";
        
        private readonly IFileSystem fileSystem;

        private readonly CommandLineOptions commandLineOptions;

        public FileSystemSourceHandler(IFileSystem fileSystem, CommandLineOptions commandLineOptions) =>
            (this.fileSystem, this.commandLineOptions) = (fileSystem, commandLineOptions);

        public Stream GetLayout()
        {
            return this.fileSystem.FileStream.Create(
                this.fileSystem.Path.Combine(this.commandLineOptions.SourceDirectory, LAYOUT_NAME), FileMode.Open);
        }

        public IEnumerable<Stream> GetPartials()
        {
            return this.fileSystem.Directory.EnumerateFiles(
                this.fileSystem.Path.Combine(this.commandLineOptions.SourceDirectory, PARIALS_DIRECTORY))
                .Select(path => this.fileSystem.FileStream.Create(path, FileMode.Open));
        }
    }
}