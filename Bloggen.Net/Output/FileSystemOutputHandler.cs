using System.IO;
using System.IO.Abstractions;
using Bloggen.Net.Model;
using Bloggen.Net.Output.Implementation;
using Bloggen.Net.Template;

namespace Bloggen.Net.Output
{
    public class FileSystemOutputHandler : IOutputHandler
    {
        public const string POSTS_DIRECTORY = "posts";

        public const string TAGS_DIRECTORY = "tags";

        private const string EXTENSION = "html";

        private readonly string outputDirectory;

        private readonly IFileSystem fileSystem;

        private readonly IContext<Post, Tag> context;

        private readonly ITemplateHandler templateHandler;

        public FileSystemOutputHandler(
            CommandLineOptions commandLineOptions,
            IFileSystem fileSystem,
            IContext<Post, Tag> context,
            ITemplateHandler templateHandler) =>
            (this.outputDirectory, this.fileSystem, this.context, this.templateHandler) =
            (commandLineOptions.OutputDirectory, fileSystem, context, templateHandler);

        public void Generate()
        {
            this.ClearOutput();

            this.GeneratePosts();

            this.GenerateTags();
        }

        private void ClearOutput()
        {
            if (this.fileSystem.Directory.Exists(this.outputDirectory))
            {
                this.fileSystem.Directory.Delete(this.outputDirectory, true);
            }

            this.fileSystem.Directory.CreateDirectory(this.outputDirectory);
        }

        private void GeneratePosts()
        {
            var postsPath = this.fileSystem.Path.Combine(this.outputDirectory, POSTS_DIRECTORY);

            this.fileSystem.Directory.CreateDirectory(postsPath);

            foreach (var p in this.context.Posts)
            {
                using var sw = this.fileSystem.File.CreateText(
                    $"{this.fileSystem.Path.Combine(postsPath, p.FileName)}.{EXTENSION}"
                );

                this.templateHandler.Write(sw, "post", p);
            }
        }

        private void GenerateTags()
        {
             var tagsPath = this.fileSystem.Path.Combine(this.outputDirectory, TAGS_DIRECTORY);

             this.fileSystem.Directory.CreateDirectory(tagsPath);

             foreach (var t in this.context.Tags)
             {
                 using var sw = this.fileSystem.File.CreateText(
                     $"{this.fileSystem.Path.Combine(tagsPath, t.Name)}.{EXTENSION}"
                 );

                 this.templateHandler.Write(sw, "tag", t);
             }
        }
    }
}