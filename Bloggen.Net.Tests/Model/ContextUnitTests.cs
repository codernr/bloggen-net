using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bloggen.Net.Model;
using Bloggen.Net.Serialization;
using Bloggen.Net.Source;
using Moq;
using Xunit;

namespace Bloggen.Net.Tests.Model
{
    public class ContextUnitTests
    {
        [Fact]
        public void ShouldPopulatePostsAndTagsCorrectly()
        {
            var service = new Context<Post, Tag>(this.GetSourceHandler(), this.GetDeserializer());

            var posts = service.Posts.ToArray();
            var tags = service.Tags.ToArray();

            Assert.Equal(3, tags.Length);
            Assert.Equal("a", posts[0].FileName);
            Assert.Equal("b", posts[1].FileName);
            Assert.Equal("tag1", tags[0].Name);
            Assert.Equal("tag2", tags[1].Name);
            Assert.Equal("tag3", tags[2].Name);
            Assert.Contains(tags[0], posts[0].TagReferences);
            Assert.Contains(tags[1], posts[0].TagReferences);
            Assert.Contains(tags[1], posts[1].TagReferences);
            Assert.Contains(tags[2], posts[1].TagReferences);
            Assert.Contains(posts[0], tags[0].PostReferences);
            Assert.Contains(posts[0], tags[1].PostReferences);
            Assert.Contains(posts[1], tags[1].PostReferences);
            Assert.Contains(posts[1], tags[2].PostReferences);
        }

        private ISourceHandler GetSourceHandler()
        {
            var m = new Mock<ISourceHandler>();

            m.Setup(o => o.GetPosts()).Returns(new[]
            {
                ("a", Stream("a")), ("b", Stream("b"))
            });

            return m.Object;
        }

        private IFrontMatterDeserializer GetDeserializer()
        {
            var m = new Mock<IFrontMatterDeserializer>();

            m.Setup(o => o.Deserialize<Post>(It.IsAny<TextReader>())).Returns(new Func<TextReader, Post>(this.GetPost));

            return m.Object;
        }

        private Post GetPost(TextReader tr)
        {
            var content = tr.ReadToEnd();
            return content switch
                {
                    "a" => new Post { Title = "a", FileName = "a", Tags = new List<string> { "tag1", "tag2" }},
                    "b" => new Post { Title = "b", FileName = "b", Tags = new List<string> { "tag2", "tag3" }},
                    _ => null!
                };
        }

        private static Stream Stream(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        private class Post : IPost
        {
            public string FileName { get; set; } = null!;
            
            public string? Title { get; set; }

            public string? Excerpt { get; set; }

            public List<string> Tags { get; set; } = new List<string>();

            public List<ITag> TagReferences { get; set; } = new List<ITag>();
        }

        private class Tag : ITag
        {
            public string? Name { get; set; }

            public List<IPost> PostReferences { get; set; } = new List<IPost>();
        }
    }
}