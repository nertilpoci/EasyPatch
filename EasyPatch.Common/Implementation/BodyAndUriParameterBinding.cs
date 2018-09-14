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

namespace EasyPatch.Common.Implementation
{
    public class BodyAndUriParameterBinding : HttpParameterBinding
    {
        public BodyAndUriParameterBinding(HttpParameterDescriptor descriptor)
            : base(descriptor)
        {
            var httpConfiguration = descriptor.Configuration;

            BodyModelValidator = httpConfiguration.Services.GetBodyModelValidator();
            Formatters = httpConfiguration.Formatters;
        }

        private readonly IBodyModelValidator BodyModelValidator;
        public event EventHandler<BoundBodyContentToModelEventArgs> BoundBodyContentToModel;
        public event EventHandler<BoundUriKeyToModelEventArgs> BoundUriKeyToModel;
        private readonly IEnumerable<MediaTypeFormatter> Formatters;
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

        // Perf-sensitive - keeping the async method as small as possible.
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
        }

        protected virtual void OnBoundBodyContentToObject(object model, Stream bodyContent)
        {
            if (BoundBodyContentToModel != null)
            {
                BoundBodyContentToModel(this, new BoundBodyContentToModelEventArgs()
                {
                    Content = bodyContent,
                    Model = model
                });
            }
        }

        protected virtual void OnBoundUriKeyToObject(object model, string key)
        {
            if (BoundUriKeyToModel != null)
            {
                BoundUriKeyToModel(this, new BoundUriKeyToModelEventArgs()
                {
                    Key = key,
                    Model = model
                });
            }
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

                // Gets our hydrated request-model.
                contentValue = await new StringContent(
                    contentString,
                    Encoding.UTF8,
                    content.Headers.ContentType?.MediaType).ReadAsAsync(type, formatters, formatterLogger, cancellationToken);

                // Reset our stream so any registered-event can read.
                onReadBodyContentStream.Seek(0, SeekOrigin.Begin);

                OnBoundBodyContentToObject(contentValue, onReadBodyContentStream);
            }

            return contentValue;
        }
    }

    public class BoundBodyContentToModelEventArgs : EventArgs
    {
        public Stream Content { get; set; }
        public object Model { get; set; }
    }

    public class BoundUriKeyToModelEventArgs : EventArgs
    {
        public string Key { get; set; }
        public object Model { get; set; }
    }
}
