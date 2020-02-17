using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bloggen.Net.Serialization;
using Bloggen.Net.Source;

namespace Bloggen.Net.Model
{
    public class Context<TPost, TTag, TPage> : IContext<TPost, TTag, TPage> where TPost : IPost, new() where TTag : ITag, new() where TPage : IPage, new()
    {
        private readonly List<TPost> posts = new List<TPost>();

        private readonly List<TTag> tags = new List<TTag>();

        private readonly List<TPage> pages = new List<TPage>();

        private readonly ISourceHandler sourceHandler;

        private readonly IFrontMatterDeserializer frontMatterDeserializer;

        public IEnumerable<TPost> Posts => this.posts;

        public IEnumerable<TTag> Tags => this.tags;

        public IEnumerable<TPage> Pages => this.pages;

        public Context(ISourceHandler sourceHandler, IFrontMatterDeserializer frontMatterDeserializer)
        {
            this.sourceHandler = sourceHandler;
            this.frontMatterDeserializer = frontMatterDeserializer;

            this.InitializePosts();
            this.InitializeTags();
            this.InitializePages();
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

        private void InitializePages()
        {
            foreach (var p in this.sourceHandler.GetPages())
            {
                using var sr = new StreamReader(p.stream);

                var page = this.frontMatterDeserializer.Deserialize<TPage>(sr);

                page.FileName = p.fileName;

                this.pages.Add(page);
            }
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