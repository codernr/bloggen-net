using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.InteropServices;
using Bloggen.Net.Source;
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
                { isWin ? @"c:\source\index.hbs" : "/source/index.hbs", new MockFileData("hello") }
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
                { isWin ? @"c:\source\layouts\page.hbs" : "/source/layouts/page.hbs", new MockFileData(string.Empty) },
                { isWin ? @"c:\source\layouts\post.hbs" : "/source/layouts/post.hbs", new MockFileData(string.Empty) },
                { isWin ? @"c:\source\layouts\archive.hbs" : "/source/layouts/archive.hbs", new MockFileData(string.Empty) },
                { isWin ? @"c:\source\layouts\tags.hbs" : "/source/layouts/tags.hbs", new MockFileData(string.Empty) }
            }));

            var layouts = service.GetLayouts().ToArray();

            Assert.Equal(4, layouts.Length);
            Assert.Equal("page", layouts[0].partialName);
            Assert.Equal("post", layouts[1].partialName);
            Assert.Equal("archive", layouts[2].partialName);
            Assert.Equal("tags", layouts[3].partialName);
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
                { isWin ? @"c:\source\partials\a.hbs" : "/source/partials/a.hbs", new MockFileData("a") },
                { isWin ? @"c:\source\partials\b.hbs" : "/source/partials/b.hbs", new MockFileData("b") }
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

        private FileSystemSourceHandler Construct(MockFileSystem mock)
        {
            return new FileSystemSourceHandler(mock, new CommandLineOptions
            {
                SourceDirectory = isWin ? @"c:\source" : "/source", OutputDirectory = isWin ? @"c:\output" : "/output"    
            });
        }
    }

}