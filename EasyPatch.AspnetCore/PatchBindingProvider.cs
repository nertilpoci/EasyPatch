using EasyPatch.Common.Interface;
using EasyPatch.Common.Settings;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EasyPatch.Common.Implementation
{


    public class EasyPatchBindingProvider : TextInputFormatter
    {
        Configuration _config;
        public EasyPatchBindingProvider(Configuration config =null)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            _config = config??new Configuration();
        }

        protected override bool CanReadType(Type type)
        {
            if(typeof(IEasyPatchModel).IsAssignableFrom(type)) return base.CanReadType(type);
            return false;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (effectiveEncoding == null)
            {
                throw new ArgumentNullException(nameof(effectiveEncoding));
            }

            var request = context.HttpContext.Request;

            using (var reader = new StreamReader(request.Body, effectiveEncoding))
            {
                try
                {
                    var rawContent = await reader.ReadToEndAsync();
                    var dictionary = JObject.Parse(rawContent);
                    var model = JsonConvert.DeserializeObject(rawContent,context.ModelType);
                    foreach (var kvp in dictionary)
                    {
                        ((IEasyPatchModel)model).AddBoundProperty(kvp.Key);
                    }
                    //custom validation from our rules
                    if (_config.PopulateModelState)
                    {
                        if (model is IEasyPatchModel state)
                        {
                            foreach (var error in state.Validate())
                            {
                                context.ModelState.AddModelError(error.Key, error.Value);
                            }
                        }
                    }
                    return await InputFormatterResult.SuccessAsync(model);
                }
                catch
                {
                    return await InputFormatterResult.FailureAsync();
                }
            }
        }

        public override Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            return base.ReadAsync(context); 
        }
    }
}
