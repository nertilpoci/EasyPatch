using EasyPatch.Common.Install;
using EasyPatch.Common.Interface;
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


    #region classdef
    public class PatchBindingProvider : TextInputFormatter
    #endregion
    {
        Configuration _config;
        public PatchBindingProvider(Configuration config=null)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            _config = config??new Configuration();
        }

        #region canreadtype
        protected override bool CanReadType(Type type)
        {
            if(typeof(IPatchState).IsAssignableFrom(type)) return base.CanReadType(type);
            return false;
        }
        #endregion

        #region readrequest
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
                        ((IPatchState)model).AddBoundProperty(kvp.Key);
                    }
                    //custom validation from our rules
                    if (_config.PopulateModelState)
                    {
                        if (model is IPatchState state)
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

        #endregion
    }
}
