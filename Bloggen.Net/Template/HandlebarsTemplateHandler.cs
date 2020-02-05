using System;
using System.Collections.Generic;
using System.IO;
using Bloggen.Net.Source;
using HandlebarsDotNet;

namespace Bloggen.Net.Template
{
    public class HandlebarsTemplateHandler : ITemplateHandler
    {
        private readonly IHandlebars handlebars;

        private readonly Action<TextWriter, object> renderLayout;

        public HandlebarsTemplateHandler(ISourceHandler sourceHandler, IHandlebars handlebars)
        {
            this.handlebars = handlebars;

            using var sr = new StreamReader(sourceHandler.GetLayout());

            this.renderLayout = this.handlebars.Compile(sr);

            this.RegisterPartials(sourceHandler.GetPartials());
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
    }
}