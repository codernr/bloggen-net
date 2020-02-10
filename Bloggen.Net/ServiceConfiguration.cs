using System;
using System.IO;
using System.IO.Abstractions;
using Bloggen.Net.Config;
using Bloggen.Net.Model;
using Bloggen.Net.Serialization;
using Bloggen.Net.Source;
using Bloggen.Net.Template;
using HandlebarsDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

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
                    .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build())
                .AddSingleton<Func<TextReader, IParser>>(tr => new Parser(tr))
                .AddSingleton<IFrontMatterDeserializer, FrontMatterDeserializer>()
                .AddSingleton<IContext, Context>()
                .AddSingleton<IHandlebars>(Handlebars.Create())
                .AddSingleton<ITemplateHandler, HandlebarsTemplateHandler>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}