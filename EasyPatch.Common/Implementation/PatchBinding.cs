using EasyPatch.Common.Install;
using EasyPatch.Common.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Http.Controllers;

namespace EasyPatch.Common.Implementation
{
    public class PatchBinding : BodyAndUriParameterBinding
    {
        public PatchBinding(HttpParameterDescriptor descriptor, Configuration config = null)
            : base(descriptor,config)
        {
            // Called once body has been parsed.
            BoundBodyContentToModel += (sender, eventArgs) =>
            {
                var rawContent = new StreamReader(eventArgs.Content).ReadToEnd();

                // We want **just** the values passed into the request.
                var dictionary =  JObject.Parse(rawContent);

                foreach (var kvp in dictionary)
                {
                    ((IPatchState)eventArgs.Model).AddBoundProperty(kvp.Key);
                }
              
            };

            // Called with every value in the RouteData collection.
            BoundUriKeyToModel += (sender, eventArgs) =>
            {
                ((IPatchState)eventArgs.Model).AddBoundProperty(eventArgs.Key);
            };
        }
    }
}
