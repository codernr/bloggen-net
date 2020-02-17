using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.InteropServices;
using Bloggen.Net.Config;
using Bloggen.Net.Source;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Bloggen.Net.Tests
{
    public class FileSystemSourceHandlerUnitTests
    {
        private static bool isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        [Fact]
        public void ShouldReturnTemplateStreamWhenExists()
        {
            var service = this.Construct(new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { isWin ? @"c:\source\templates\x\index.hbs" : "/source/templates/x/index.hbs", new MockFileData("hello") }
            }));

            Assert.IsAssignableFrom<Stream>(service.GetTemplate());
        }

        [Fact]
        public void ShouldThrowWhenTemplateDoesntExist()
        {
            var service = this.Construct(new MockFileSystem());

            Assert.ThrowsAny<IOException>(() => service.GetTemplate());
        }

        [Fact]
        public void ShouldReturnLayoutStreams()
        {
            var service = this.Construct(new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { isWin ? @"c:\source\templates\x\layouts\page.hbs" : "/source/templates/x/layouts/page.hbs", new MockFileData(string.Empty) },
                { isWin ? @"c:\source\templates\x\layouts\post.hbs" : "/source/templates/x/layouts/post.hbs", new MockFileData(string.Empty) },
                { isWin ? @"c:\source\templates\x\layouts\archive.hbs" : "/source/templates/x/layouts/archive.hbs", new MockFileData(string.Empty) },
                { isWin ? @"c:\source\templates\x\layouts\tags.hbs" : "/source/templates/x/layouts/tags.hbs", new MockFileData(string.Empty) },
                { isWin ? @"c:\source\templates\x\layouts\tag.hbs" : "/source/templates/x/layouts/tag.hbs", new MockFileData(string.Empty) }
            }));

            var layouts = service.GetLayouts().ToArray();

            Assert.Equal(5, layouts.Length);
            Assert.Equal("page", layouts[0].partialName);
            Assert.Equal("post", layouts[1].partialName);
            Assert.Equal("archive", layouts[2].partialName);
            Assert.Equal("tags", layouts[3].partialName);
            Assert.Equal("tag", layouts[4].partialName);
        }

        [Fact]
        public void ShouldThrowWhenLayoutIsNotPresent()
        {
            var service = this.Construct(new MockFileSystem());

            Assert.ThrowsAny<IOException>(() => service.GetLayouts());
        }

        [Fact]
        public void ShouldReturnPartialStreams()
        {
            var service = this.Construct(new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { isWin ? @"c:\source\templates\x\partials\a.hbs" : "/source/templates/x/partials/a.hbs", new MockFileData("a") },
                { isWin ? @"c:\source\templates\x\partials\b.hbs" : "/source/templates/x/partials/b.hbs", new MockFileData("b") }
            }));

            var partials = service.GetPartials().ToArray();
            
            Assert.Equal(2, partials.Count());
            Assert.Equal("a", partials[0].partialName);
            Assert.Equal("b", partials[1].partialName);
        }

        [Fact]
        public void ShouldThrowWhenPartialsDirNotPresent()
        {
            var service = this.Construct(new MockFileSystem());

            Assert.Throws<DirectoryNotFoundException>(() => service.GetPartials());
        }

        [Fact]
        public void ShouldReturnPostsStreams()
        {
            var service = this.Construct(new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { isWin ? @"c:\source\posts\a.hbs" : "/source/posts/a.hbs", new MockFileData("a") },
                { isWin ? @"c:\source\posts\b.hbs" : "/source/posts/b.hbs", new MockFileData("b") }
            }));

            var posts = service.GetPosts().ToArray();
            
            Assert.Equal(2, posts.Count());
            Assert.Equal("a", posts[0].fileName);
            Assert.Equal("b", posts[1].fileName);
        }

        [Fact]
        public void ShouldThrowWhenPostsDirNotPresent()
        {
            var service = this.Construct(new MockFileSystem());

            Assert.Throws<DirectoryNotFoundException>(() => service.GetPartials());
        }

        private FileSystemSourceHandler Construct(MockFileSystem mock)
        {
            var optionsMock = new Mock<IOptions<SiteConfig>>();

            optionsMock.Setup(m => m.Value).Returns(new SiteConfig { Template = "x" });

            return new FileSystemSourceHandler(mock, new CommandLineOptions
            {
                SourceDirectory = isWin ? @"c:\source" : "/source", OutputDirectory = isWin ? @"c:\output" : "/output"    
            }, optionsMock.Object);
        }
    }

}