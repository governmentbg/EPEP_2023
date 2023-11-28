using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Epep.Web.Extensions.ModelBinders
{
    /// <summary>
    /// Author: Stamo Petkov
    /// Created: 07.01.2017
    /// Description: Корекция на формата на датата
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {

        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value != ValueProviderResult.None && !String.IsNullOrEmpty(value.FirstValue))
            {
                string valueAsString = value.FirstValue;

                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
                decimal? result = null;
                bool success = false;
                if (!string.IsNullOrEmpty(valueAsString))
                {
                    try
                    {
                        //valueAsString = valueAsString.Trim().Replace(",", ".");
                        //System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator = ".";
                        result = decimal.Parse(valueAsString, CultureInfo.InvariantCulture);
                        success = true;
                    }
                    catch (Exception e)
                    {
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, e, bindingContext.ModelMetadata);
                    }
                }

                if (success)
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
