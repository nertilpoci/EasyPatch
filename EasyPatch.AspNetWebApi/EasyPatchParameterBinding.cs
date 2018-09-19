using EasyPatch.Common.Settings;
using EasyPatch.Common.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.Validation;

namespace EasyPatch.WebApi2
{ 
    public class EasyPatchParameterBinding : HttpParameterBinding
    {
        private readonly Configuration configuration;
        private readonly IBodyModelValidator BodyModelValidator;
        private readonly IEnumerable<MediaTypeFormatter> Formatters;
        public EasyPatchParameterBinding(HttpParameterDescriptor descriptor, Configuration config = null)
          : base(descriptor)
        {
            var httpConfiguration = descriptor.Configuration;

            BodyModelValidator = httpConfiguration.Services.GetBodyModelValidator();
            Formatters = httpConfiguration.Formatters;
            configuration = config ?? new Configuration();
        }
        public override bool WillReadBody => true;

        public override Task ExecuteBindingAsync(
            ModelMetadataProvider metadataProvider,
            HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            var paramFromBody = Descriptor;
            var type = paramFromBody.ParameterType;
            var request = actionContext.ControllerContext.Request;
            var formatterLogger = new ModelStateFormatterLogger(actionContext.ModelState, paramFromBody.ParameterName);
            return ExecuteBindingAsyncCore(metadataProvider, actionContext, paramFromBody, type, request, formatterLogger, cancellationToken);
        }

        private async Task ExecuteBindingAsyncCore(
            ModelMetadataProvider metadataProvider,
            HttpActionContext actionContext,
            HttpParameterDescriptor paramFromBody,
            Type type,
            HttpRequestMessage request,
            IFormatterLogger formatterLogger,
            CancellationToken cancellationToken)
        {
            var model = await ReadContentAsync(request, type, Formatters, formatterLogger, cancellationToken);

            if (model != null)
            {
                var routeParams = actionContext.ControllerContext.RouteData.Values;

                foreach (var key in routeParams.Keys.Where(k => k != "controller"))
                {
                    var prop = type.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                    if (prop == null)
                    {
                        continue;
                    }

                    var descriptor = TypeDescriptor.GetConverter(prop.PropertyType);

                    if (descriptor.CanConvertFrom(typeof(string)))
                    {
                        prop.SetValue(model, descriptor.ConvertFromString(routeParams[key] as string));

                        OnBoundUriKeyToObject(model, key);
                    }
                }
            }

            // Set the merged model in the context.
            SetValue(actionContext, model);

            if (BodyModelValidator != null)
            {
                BodyModelValidator.Validate(model, type, metadataProvider, actionContext, paramFromBody.ParameterName);
            }

            //custom validation from our rules
            if (configuration.PopulateModelState)
            {
                if (model is IEasyPatchModel state)
                {
                    foreach (var error in state.Validate())
                    {
                        actionContext.ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }
        }

        protected void OnBoundBodyContentToObject(object model, Stream bodyContent)
        {
            var rawContent = new StreamReader(bodyContent).ReadToEnd();

            var dictionary = JObject.Parse(rawContent);

            foreach (var kvp in dictionary)
            {
                ((IEasyPatchModel)model).AddBoundProperty(kvp.Key);
            }
        }

        protected  void OnBoundUriKeyToObject(object model, string key)
        {
             ((IEasyPatchModel)model).AddBoundProperty(key);
        }

        private async Task<object> ReadContentAsync(
            HttpRequestMessage request,
            Type type,
            IEnumerable<MediaTypeFormatter> formatters,
            IFormatterLogger formatterLogger,
            CancellationToken cancellationToken)
        {
            var content = request.Content;

            if (content == null)
            {
                var defaultValue = MediaTypeFormatter.GetDefaultValueForType(type);

                return defaultValue == null ? Task.FromResult<object>(null) : Task.FromResult(defaultValue);
            }

            object contentValue = null;

            using (var onReadBodyContentStream = new MemoryStream())
            {
                var contentStream = await content.ReadAsStreamAsync();
                contentStream.CopyTo(onReadBodyContentStream);
                onReadBodyContentStream.Seek(0, SeekOrigin.Begin);

                var contentString = new StreamReader(onReadBodyContentStream).ReadToEnd();

                contentValue = await new StringContent(
                    contentString,
                    Encoding.UTF8,
                    content.Headers.ContentType?.MediaType).ReadAsAsync(type, formatters, formatterLogger, cancellationToken);

                onReadBodyContentStream.Seek(0, SeekOrigin.Begin);

                OnBoundBodyContentToObject(contentValue, onReadBodyContentStream);
            }

            return contentValue;
        }

    }
}
