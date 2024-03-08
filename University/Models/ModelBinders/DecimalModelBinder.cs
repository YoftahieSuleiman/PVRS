using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace University.Models.ModelBinders
{
   
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            try
            {
                return valueProviderResult == null ? base.BindModel(controllerContext, bindingContext) : Math.Round(Convert.ToDecimal(valueProviderResult.AttemptedValue), 6);

            }
            catch
            {
                return valueProviderResult;
            }
            // of course replace with your custom conversion logic
        }
    }

}