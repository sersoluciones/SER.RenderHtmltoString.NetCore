using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using SER.RenderHtmltoString.NetCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SER.RenderHtmltoString.NetCore.Implementation
{
    public class RazorPartialToStringRenderer : IRazorPartialToStringRenderer
    {
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly IRazorPageActivator _activator;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IActionContextAccessor _actionContext;

        public RazorPartialToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IRazorViewEngine razorViewEngine,
            IRazorPageActivator activator,
            IHttpContextAccessor httpContext,
            IActionContextAccessor actionContext)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _razorViewEngine = razorViewEngine;
            _activator = activator;
            _httpContext = httpContext;
            _actionContext = actionContext;
        }
        public async Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model)
        {
            ActionContext actionContext;

            try
            {
                actionContext = new ActionContext(
                        _httpContext.HttpContext,
                        _httpContext.HttpContext.GetRouteData(),
                        _actionContext.ActionContext.ActionDescriptor
                    );
            }
            catch (Exception)
            {
                actionContext = GetActionContext();
            }
            // var partial = FindView(actionContext, partialName);
            using var output = new StringWriter();
            var result = _razorViewEngine.FindPage(actionContext, partialName);

            if (result.Page == null)
            {
                foreach (var item in result.SearchedLocations)
                {
                    Console.WriteLine(item);
                }

                throw new ArgumentNullException($"The page {partialName} cannot be found.");
            }
            var partial = new RazorView(_razorViewEngine,
                _activator,
                new List<IRazorPage>(),
                result.Page,
                HtmlEncoder.Default,
                new DiagnosticListener("ViewRenderService"));

            var viewContext = new ViewContext(
                actionContext,
                partial,
                new ViewDataDictionary<TModel>(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions()
            );
            await partial.RenderAsync(viewContext);
            return output.ToString();
        }
        private IView FindView(ActionContext actionContext, string partialName)
        {
            var getPartialResult = _viewEngine.GetView(null, partialName, false);
            if (getPartialResult.Success)
            {
                return getPartialResult.View;
            }
            var findPartialResult = _viewEngine.FindView(actionContext, partialName, false);
            if (findPartialResult.Success)
            {
                return findPartialResult.View;
            }
            var searchedLocations = getPartialResult.SearchedLocations.Concat(findPartialResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations)); ;
            throw new InvalidOperationException(errorMessage);
        }
        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }

}