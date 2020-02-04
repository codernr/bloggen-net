using System;
using Bloggen.Net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bloggen.Net
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServiceProvider(IConfigurationRoot siteConfiguration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.Configure<SiteConfig>(siteConfiguration);

            return serviceCollection.BuildServiceProvider();
        }
    }
}