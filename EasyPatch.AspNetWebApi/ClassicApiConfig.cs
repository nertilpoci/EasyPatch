using EasyPatch.Common.Settings;
using EasyPatch.Common.Interface;

using System.Web.Http;

namespace EasyPatch.WebApi2
{
   public static class ClassicApiConfig
    {
      public static void UseEasyPatch(this HttpConfiguration config, Configuration configuration=null)
        {
            config.ParameterBindingRules.Insert(0, descriptor =>
                                   typeof(IEasyPatchModel).IsAssignableFrom(descriptor.ParameterType)
                                ? new EasyPatchParameterBinding(descriptor, configuration)
                                      : null);
        }
    }
}
