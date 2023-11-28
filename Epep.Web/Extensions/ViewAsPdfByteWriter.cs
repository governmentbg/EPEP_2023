using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace SMART.Extensions
{
    public class ViewAsPdfByteWriter : ViewAsPdf
    {
        public ViewAsPdfByteWriter(string viewName, object model, bool addPaging = false, string footerUrl = null) : base(viewName, model)
        {
            PageOrientation = Orientation.Portrait;
            PageSize = Size.A4;

            this.CustomSwitches = "--disable-smart-shrinking --margin-top 16mm --margin-right 12mm  --margin-left 25mm";
            if (addPaging || !string.IsNullOrEmpty(footerUrl))
            {
                this.CustomSwitches += " --margin-bottom 20mm --page-offset 0  --encoding utf-8";
                if (!string.IsNullOrEmpty(footerUrl))
                {
                    var pageInfo = "";
                    if (addPaging)
                    {
                        pageInfo = "--footer-right [page]";
                    }
                    this.CustomSwitches += $" --margin-bottom 20mm --page-offset 0 --footer-html \"{footerUrl}\" {pageInfo} ";
                }
                else
                {
                    if (addPaging)
                    {
                        this.CustomSwitches += " --footer-center [page]";
                    }
                }
                //this.CustomSwitches += " --footer-font-size 12 --footer-font-name \"Times New Roman\" --footer-spacing 6";
                this.CustomSwitches += " --footer-font-size 12  --footer-spacing 6";
            }
            else
            {
                this.CustomSwitches += " --margin-bottom 10mm";
            }
        }

        public async Task<byte[]> GetByte(ControllerContext context)
        {
            base.PrepareResponse(context.HttpContext.Response);
            base.ExecuteResult(context);

            return await base.BuildFile(context).ConfigureAwait(false);
        }
    }
}
