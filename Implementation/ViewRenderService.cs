using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SER.RenderHtmltoString.NetCore.Render
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IActionContextAccessor _actionContext;
        private readonly IRazorPageActivator _activator;


        public ViewRenderService(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContext,
            IRazorPageActivator activator,
            IActionContextAccessor actionContext)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;

            _httpContext = httpContext;
            _actionContext = actionContext;
            _activator = activator;

        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(),  new ActionDescriptor());
        }

        public async Task<string> RenderToStringAsync<T>(string pageName, T model) where T : PageModel
        {
            ActionContext actionContext;
            TempDataDictionary dataDict;

            try
            {
                actionContext = new ActionContext(
                        _httpContext.HttpContext,
                        _httpContext.HttpContext.GetRouteData(),
                        _actionContext.ActionContext.ActionDescriptor
                    );
                dataDict = new TempDataDictionary(
                     _httpContext.HttpContext,
                     _tempDataProvider
                 );
            }
            catch (Exception)
            {
                actionContext = GetActionContext();
                dataDict = new TempDataDictionary(
                     actionContext.HttpContext,
                     _tempDataProvider
                 );
            }

            using var sw = new StringWriter();
            var result = _razorViewEngine.FindPage(actionContext, pageName);

            if (result.Page == null)
            {
                foreach (var item in result.SearchedLocations)
                {
                    Console.WriteLine(item);
                }

                throw new ArgumentNullException($"The page {pageName} cannot be found.");
            }

            var view = new RazorView(_razorViewEngine,
                _activator,
                new List<IRazorPage>(),
                result.Page,
                HtmlEncoder.Default,
                new DiagnosticListener("ViewRenderService"));


            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<T>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                dataDict,
                sw,
                new HtmlHelperOptions()
            );

            var page = result.Page;

            page.ViewContext = new ViewContext
            {
                ViewData = viewContext.ViewData
            };

            page.ViewContext = viewContext;


            _activator.Activate(page, viewContext);

            await page.ExecuteAsync();


            return sw.ToString();
        }

        private IView FindView(ActionContext actionContext, string partialName)
        {
            var getPartialResult = _razorViewEngine.GetView(null, partialName, false);
            if (getPartialResult.Success)
            {
                return getPartialResult.View;
            }
            var findPartialResult = _razorViewEngine.FindView(actionContext, partialName, false);
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
    }
}
