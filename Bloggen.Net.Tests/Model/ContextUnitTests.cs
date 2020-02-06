using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bloggen.Net.Model;
using Bloggen.Net.Source;
using Moq;
using Xunit;
using YamlDotNet.Serialization;

namespace Bloggen.Net.Tests.Model
{
    public class ContextUnitTests
    {
        [Fact]
        public void ShouldPopulatePostsAndTagsCorrectly()
        {
            var service = new Context(this.GetSourceHandler(), this.GetDeserializer());

            var posts = service.Posts.ToArray();
            var tags = service.Tags.ToArray();

            Assert.Equal(3, tags.Length);
            Assert.Equal("a", posts[0].FileName);
            Assert.Equal("b", posts[1].FileName);
            Assert.Equal("tag1", tags[0].Name);
            Assert.Equal("tag2", tags[1].Name);
            Assert.Equal("tag3", tags[2].Name);
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

        private IDeserializer GetDeserializer()
        {
            var m = new Mock<IDeserializer>();

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
    }
}