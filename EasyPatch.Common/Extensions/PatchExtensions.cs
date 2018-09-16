using EasyPatch.Common.Implementation;
using EasyPatch.Common.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http.ModelBinding;

namespace EasyPatch.Common.Extensions
{
    public static class PatchExtensions
    {
        public static ModelStateDictionary ModelState<TRequest,TModel>(this AbstractPatchStateRequest<TRequest,TModel> request)
        {
            var state = new ModelStateDictionary() { };
            foreach (var error in request.Validate(request))
            {
                state.AddModelError(error.Key, error.Value);
            }
            return state;
        }
    }
}
