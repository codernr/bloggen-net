using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Bloggen.Net.Source;
using Xunit;

namespace Bloggen.Net.Tests
{
    public class FileSystemSourceHandlerUnitTests
    {
        [Fact]
        public void ShouldReturnLayoutStreamWhenExists()
        {
            var service = this.Construct(new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\source\layout.hbs", new MockFileData("hello") }
            }));

            Assert.IsAssignableFrom<Stream>(service.GetLayout());
        }

        [Fact]
        public void ShouldThrowWhenLayoutDoesntExist()
        {
            var service = this.Construct(new MockFileSystem());

            Assert.ThrowsAny<IOException>(() => service.GetLayout());
        }

        [Fact]
        public void ShouldReturnPartialStreams()
        {
            var service = this.Construct(new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\source\partials\a.hbs", new MockFileData("a") },
                { @"c:\source\partials\b.hbs", new MockFileData("b") }
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
                SourceDirectory = @"c:\source", OutputDirectory = @"c:\output"    
            });
        }
    }

}