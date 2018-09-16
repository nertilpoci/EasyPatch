using EasyPatch.Common.Implementation;
using EasyPatch.Common.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;

namespace EasyPatch.Common.Install
{
   public static class ClassicApiConfig
    {
      public static void UseEasyPatch(this HttpConfiguration config, Configuration configuration=null)
        {
            config.ParameterBindingRules.Insert(0, descriptor =>
                                   typeof(IPatchState).IsAssignableFrom(descriptor.ParameterType)
                                ? new PatchBinding(descriptor, configuration)
                                      : null);
        }
    }
}
