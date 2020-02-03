using System;
using Microsoft.Extensions.DependencyInjection;

namespace Bloggen.Net
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            return serviceCollection.BuildServiceProvider();
        }
    }
}