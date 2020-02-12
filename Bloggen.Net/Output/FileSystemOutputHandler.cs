using System.IO.Abstractions;

namespace Bloggen.Net.Output
{
    public class FileSystemOutputHandler : IOutputHandler
    {
        private const string POSTS_DIRECTORY = "posts";

        private const string TAGS_DIRECTORY = "tags";

        private readonly string outputDirectory;

        private readonly IFileSystem fileSystem;

        public FileSystemOutputHandler(CommandLineOptions commandLineOptions, IFileSystem fileSystem) =>
            (this.outputDirectory, this.fileSystem) = (commandLineOptions.OutputDirectory, fileSystem);

        public void Generate()
        {
            this.ClearOutput();
        }

        private void ClearOutput()
        {
            if (this.fileSystem.Directory.Exists(this.outputDirectory))
            {
                this.fileSystem.Directory.Delete(this.outputDirectory, true);
            }

            this.fileSystem.Directory.CreateDirectory(this.outputDirectory);
        }
    }
}