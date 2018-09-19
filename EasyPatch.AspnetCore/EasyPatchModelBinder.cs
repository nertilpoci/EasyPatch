using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Threading.Tasks;
namespace EasyPatch.AspnetCore
{
    public class EasyPatchModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //if (typeof(Common.Interface.IEasyPatchModel).IsAssignableFrom(  context.Metadata.ModelType))
            //{
                return new BinderTypeModelBinder(typeof(EasyPatchModelBinder));
            //}

            return null;
        }
    }
    public class EasyPatchModelBinder :IModelBinder
    {
        public EasyPatchModelBinder()
        {
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var values = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            if (values.Length == 0) return Task.CompletedTask;

            // Attempt to parse
            var stringValue = values.FirstValue;
            OrderExpression expression;
            if (OrderExpression.TryParse(stringValue, out expression))
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, expression, stringValue);
                bindingContext.Result = ModelBindingResult.Success(expression);
            }

            return Task.CompletedTask;
        }
    }
    public class OrderExpression
    {
        public string RawValue { get; set; }
        public static bool TryParse(string value, out OrderExpression expr)
        {
            expr = new OrderExpression { RawValue = value };
            return true;
        }
    }
}
