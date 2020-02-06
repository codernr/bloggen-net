using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bloggen.Net.Serialization;
using Bloggen.Net.Source;
using YamlDotNet.Serialization;

namespace Bloggen.Net.Model
{
    public class Context : IContext
    {
        private readonly List<Post> posts = new List<Post>();

        private readonly List<Tag> tags = new List<Tag>();

        private readonly ISourceHandler sourceHandler;

        private readonly IFrontMatterDeserializer frontMatterDeserializer;

        public IEnumerable<Post> Posts
        {
            get { return this.posts; }
        }

        public IEnumerable<Tag> Tags
        {
            get { return this.tags; }
        }

        public Context(ISourceHandler sourceHandler, IFrontMatterDeserializer frontMatterDeserializer)
        {
            this.sourceHandler = sourceHandler;
            this.frontMatterDeserializer = frontMatterDeserializer;

            this.InitializePosts();
            this.InitializeTags();
            this.ReferenceObjects();
        }

        private void InitializePosts()
        {
            foreach (var p in this.sourceHandler.GetPosts())
            {
                using var sr = new StreamReader(p.stream);

                var post = this.frontMatterDeserializer.Deserialize<Post>(sr);

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

        private void ReferenceObjects()
        {
            foreach (var p in this.posts)
            {
                this.CrossReferenceTags(p);
            }
        }

        private void CrossReferenceTags(Post p)
        {
            foreach (var tagName in p.Tags)
            {
                var tag = this.tags.First(t => t.Name == tagName);
                tag.PostReferences.Add(p);
                p.TagReferences.Add(tag);
            }
        }
    }
}