using System;
using System.Collections.Generic;
using System.IO;
using Bloggen.Net.Config;
using Bloggen.Net.Source;
using HandlebarsDotNet;
using Microsoft.Extensions.Options;

namespace Bloggen.Net.Template
{
    public class HandlebarsTemplateHandler : ITemplateHandler
    {
        private readonly SiteConfig siteConfig;

        private readonly IHandlebars handlebars;

        private readonly Action<TextWriter, object> renderTemplate;

        public HandlebarsTemplateHandler(ISourceHandler sourceHandler, IHandlebars handlebars, IOptions<SiteConfig> siteConfig)
        {
            this.siteConfig = siteConfig.Value;
            
            this.handlebars = handlebars;

            using var sr = new StreamReader(sourceHandler.GetTemplate());

            this.renderTemplate = this.handlebars.Compile(sr);

            this.RegisterPartials(sourceHandler.GetPartials());

            this.RegisterPartials(sourceHandler.GetLayouts());
        }

        private void RegisterPartials(IEnumerable<(string templateName, Stream stream)> partials)
        {
           foreach (var partial in partials)
           {
               using var sr = new StreamReader(partial.stream);

               var compiled = this.handlebars.Compile(sr);

               this.handlebars.RegisterTemplate(partial.templateName, compiled);
           }
        }

        public void Write(TextWriter writer, string layout, object data)
        {
            this.renderTemplate(writer, new { layout = layout, data = data, site = this.siteConfig });
        }
    }
}