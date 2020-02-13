using System;
using System.IO;
using Bloggen.Net.Output;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bloggen.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(Build)
                .WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });
        }

        static void Build(CommandLineOptions options)
        {
            var siteConfiguration = new ConfigurationBuilder()
                .AddYamlFile(Path.Combine(options.SourceDirectory, "config.yml"), false)
                .Build();

            var serviceProvider = ServiceConfiguration.ConfigureServiceProvider(siteConfiguration, options);

            serviceProvider.GetService<IOutputHandler>().Generate();
        }
    }
}
