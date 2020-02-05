using System;
using System.Collections.Generic;
using System.IO;
using Bloggen.Net.Source;
using HandlebarsDotNet;

namespace Bloggen.Net.Template
{
    public class HandlebarsTemplateHandler : ITemplateHandler
    {
        private readonly Action<TextWriter, object> renderLayout;

        public HandlebarsTemplateHandler(ISourceHandler sourceHandler)
        {
            using var sr = new StreamReader(sourceHandler.GetLayout());

            this.renderLayout = Handlebars.Compile(sr);

            RegisterPartials(sourceHandler.GetPartials());
        }

        private static void RegisterPartials(IEnumerable<(string templateName, Stream stream)> partials)
        {
           foreach (var partial in partials)
           {
               using var sr = new StreamReader(partial.stream);

               var compiled = Handlebars.Compile(sr);

               Handlebars.RegisterTemplate(partial.templateName, compiled);
           }
        }
    }
}