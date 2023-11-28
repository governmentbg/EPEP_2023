using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using RazorEngine.Templating;

namespace eCase.Web.Jobs
{
    public class RazorEngineRenderer
    {
        private static IRazorEngineService razorEngineService = RazorEngineService.Create();
        private static HashSet<string> compiledTemplates = new HashSet<string>();

        public static string RenderTemplate(string templateFileName, JObject context)
        {
            if (!RazorEngineRenderer.compiledTemplates.Contains(templateFileName))
            {
                string templatePath = GetTemplatePath(templateFileName);
                string razorTemplate = File.ReadAllText(templatePath);
                RazorEngineRenderer.razorEngineService.Compile(razorTemplate, templateFileName, typeof(JObject));
                RazorEngineRenderer.compiledTemplates.Add(templateFileName);
            }

            return RazorEngineRenderer.razorEngineService.Run(templateFileName, null, context);
        }

        private static string GetTemplatePath(string templateName)
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string binPath = System.IO.Path.GetDirectoryName(rootPath);
            string templateFullPath = String.Format(@"{0}\Jobs\Templates\{1}", binPath, templateName);

            return templateFullPath;
        }
    }
}