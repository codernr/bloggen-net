using System;
using System.IO;
using Bloggen.Net.Source;
using HandlebarsDotNet;

namespace Bloggen.Net.Template
{
    public class HandlebarsTemplateHandler : ITemplateHandler
    {
        private readonly ISourceHandler sourceHandler;
        private readonly Action<TextWriter, object> renderLayout;

        public HandlebarsTemplateHandler(ISourceHandler sourceHandler)
        {
            this.sourceHandler = sourceHandler;

            using var sr = new StreamReader(this.sourceHandler.GetLayout());

            this.renderLayout = Handlebars.Compile(sr);
        }
    }
}