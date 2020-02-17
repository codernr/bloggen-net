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

        private readonly Dictionary<string, Action<TextWriter, object>> layouts = new Dictionary<string, Action<TextWriter, object>>();

        public HandlebarsTemplateHandler(
            ISourceHandler sourceHandler,
            IHandlebars handlebars,
            IOptions<SiteConfig> siteConfig)
        {
            this.siteConfig = siteConfig.Value;
            
            this.handlebars = handlebars;

            using var sr = new StreamReader(sourceHandler.GetTemplate());

            this.RegisterHelpers();

            this.renderTemplate = this.handlebars.Compile(sr);

            this.RegisterPartials(sourceHandler.GetPartials());

            this.RegisterLayouts(sourceHandler.GetLayouts());
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

        private void RegisterLayouts(IEnumerable<(string layoutName, Stream stream)> layouts)
        {
            foreach(var layout in layouts)
            {
                using var sr = new StreamReader(layout.stream);

                this.layouts.Add(layout.layoutName, this.handlebars.Compile(sr));
            }
        }

        private void RegisterHelpers()
        {
            this.handlebars.RegisterHelper("render", (writer, context, parameters) => 
            {
                if (parameters.Length != 1 || !(parameters[0] is string) || parameters[0] == null)
                {
                    throw new ArgumentException("Layout helper needs exactly one string parameter");
                }

                string layout = (parameters[0] as string)!;

                this.layouts[layout](writer, context);
            });
        }

        public void Write(TextWriter writer, string layout, object data, string? content = null)
        {
            this.renderTemplate(writer, new { layout = layout, data = data, site = this.siteConfig, content = content });
        }
    }
}