﻿using EasyPatch.Common.Implementation;
using EasyPatch.Common.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyPatch.AspnetCore
{
    public static class CoreApiConfig
    {
        public static void UseEasyPatch(this MvcOptions options, Configuration configuration = null)
        {
            options.InputFormatters.Insert(0, new EasyPatchBindingProvider(configuration));
        }
    }
}
