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
        public void ShouldReturnLayoutStreamWhenExists()
        {
            var service = this.Construct(new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { isWin ? @"c:\source\index.hbs" : "/source/index.hbs", new MockFileData("hello") }
            }));

            Assert.IsAssignableFrom<Stream>(service.GetTemplate());
        }

        [Fact]
        public void ShouldThrowWhenLayoutDoesntExist()
        {
            var service = this.Construct(new MockFileSystem());

            Assert.ThrowsAny<IOException>(() => service.GetTemplate());
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