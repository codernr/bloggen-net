using System;
using System.IO.Abstractions;
using Bloggen.Net.Config;
using Bloggen.Net.Model;
using Bloggen.Net.Source;
using Bloggen.Net.Template;
using HandlebarsDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
                .AddSingleton<IDeserializer>(new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build())
                .AddSingleton<IContext, Context>()
                .AddSingleton<IHandlebars>(Handlebars.Create())
                .AddSingleton<ITemplateHandler, HandlebarsTemplateHandler>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}