using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bloggen.Net.Serialization;
using Bloggen.Net.Source;

namespace Bloggen.Net.Model
{
    public class Context<TPost, TTag> : IContext<TPost, TTag> where TPost : IPost, new() where TTag : ITag, new()
    {
        private readonly List<TPost> posts = new List<TPost>();

        private readonly List<TTag> tags = new List<TTag>();

        private readonly ISourceHandler sourceHandler;

        private readonly IFrontMatterDeserializer frontMatterDeserializer;

        public IEnumerable<TPost> Posts
        {
            get { return this.posts; }
        }

        public IEnumerable<TTag> Tags
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

                var post = this.frontMatterDeserializer.Deserialize<TPost>(sr);

                post.FileName = p.fileName;

                this.posts.Add(post);
            }
        }

        private void InitializeTags()
        {
            this.tags.AddRange(
                this.posts.SelectMany(p => p.Tags)
                    .Distinct()
                    .Select(t => new TTag { Name = t }));
        }

        private void ReferenceObjects()
        {
            foreach (var p in this.posts)
            {
                this.CrossReferenceTags(p);
            }
        }

        private void CrossReferenceTags(TPost p)
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