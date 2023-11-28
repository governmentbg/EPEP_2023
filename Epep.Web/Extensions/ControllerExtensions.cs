using Epep.Core.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Epep.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static void AppendErrors(this ModelStateDictionary modelState ,SaveResultVM result)
        {
            if(modelState.IsValid)
            {
                return;
            }

            foreach (var item in modelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    result.AddError(error.ErrorMessage, item.Key);
                }
            }
        }
    }
}
