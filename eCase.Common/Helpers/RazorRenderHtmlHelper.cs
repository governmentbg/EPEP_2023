using RazorEngine;
using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using RazorEngine.Templating;

namespace eCase.Common.Helpers
{
    public class RazorRenderHtmlHelper
    {
        #region Public

        public static string RenderHtml(object model, string viewTemplate)
        {
            string htmlTemplate = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(viewTemplate));

#if DEBUG
            var debugName = Guid.NewGuid().ToString();
            Engine.Razor
                .AddTemplate
                    (
                        new RazorEngine.Templating.NameOnlyTemplateKey(debugName, RazorEngine.Templating.ResolveType.Global, null), 
                        new LoadedTemplateSource(htmlTemplate)
                    );
            return Engine.Razor.RunCompile(debugName, model.GetType(), model);
#else
            Engine.Razor.AddTemplate(viewTemplate, htmlTemplate);
            return Engine.Razor.RunCompile(viewTemplate, model.GetType(), model);
#endif
        }

        public static string RenderHtml(Controller controller, object model, string viewTemplate)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewTemplate);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string ConvertTextToHtml(string text)
        {
            //Create a StringBuilder object from the string intput
            //parameter
            StringBuilder sb = new StringBuilder(text);
            //Replace all double white spaces with a single white space
            //and &nbsp;
            sb.Replace("  ", " &nbsp;");
            //Check if HTML tags are not allowed
            //Convert the brackets into HTML equivalents
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            //Convert the double quote
            sb.Replace("\"", "&quot;");
            //Create a StringReader from the processed string of 
            //the StringBuilder object
            StringReader sr = new StringReader(sb.ToString());
            StringWriter sw = new StringWriter();
            //Loop while next character exists
            while (sr.Peek() > -1)
            {
                //Read a line from the string and store it to a temp
                //variable
                string temp = sr.ReadLine();
                //write the string with the HTML break tag
                //Note here write method writes to a Internal StringBuilder
                //object created automatically
                sw.Write(temp + "<br>");
            }
            //Return the final processed text
            return sw.GetStringBuilder().ToString();
        }

        #endregion
    }
}
