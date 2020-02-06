using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bloggen.Net.Source;
using YamlDotNet.Serialization;

namespace Bloggen.Net.Model
{
    public class Context : IContext
    {
        private readonly List<Post> posts = new List<Post>();

        private readonly List<Tag> tags = new List<Tag>();

        private readonly ISourceHandler sourceHandler;

        private readonly IDeserializer deserializer;

        public IEnumerable<Post> Posts
        {
            get { return this.posts; }
        }

        public IEnumerable<Tag> Tags
        {
            get { return this.tags; }
        }

        public Context(ISourceHandler sourceHandler, IDeserializer deserializer)
        {
            this.sourceHandler = sourceHandler;
            this.deserializer = deserializer;

            this.InitializePosts();
            this.InitializeTags();
        }

        private void InitializePosts()
        {
            foreach (var p in this.sourceHandler.GetPosts())
            {
                using var sr = new StreamReader(p.stream);

                var post = this.deserializer.Deserialize<Post>(sr);

                post.FileName = p.fileName;

                this.posts.Add(post);
            }
        }

        private void InitializeTags()
        {
            this.tags.AddRange(
                this.posts.SelectMany(p => p.Tags)
                    .Distinct()
                    .Select(t => new Tag { Name = t }));
        }
    }
}