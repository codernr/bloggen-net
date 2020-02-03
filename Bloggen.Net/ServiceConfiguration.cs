using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bloggen.Net
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServiceProvider(IConfigurationRoot configuration)
        {
            var serviceCollection = new ServiceCollection();

            return serviceCollection.BuildServiceProvider();
        }
    }
}