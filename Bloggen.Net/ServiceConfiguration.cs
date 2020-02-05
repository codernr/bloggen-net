using System;
using System.IO.Abstractions;
using Bloggen.Net.Config;
using Bloggen.Net.Source;
using Bloggen.Net.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bloggen.Net
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServiceProvider(IConfigurationRoot siteConfiguration, CommandLineOptions commandLineOptions)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton(commandLineOptions)
                .Configure<SiteConfig>(siteConfiguration)
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<ISourceHandler, FileSystemSourceHandler>()
                .AddSingleton<ITemplateHandler, HandlebarsTemplateHandler>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}