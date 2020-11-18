using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SER.RenderHtmltoString.NetCore.Implementation;
using SER.RenderHtmltoString.NetCore.Interfaces;
using SER.RenderHtmltoString.NetCore.Render;
using SER.RenderHtmltoString.NetCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SER.RenderHtmltoString.NetCore.Configuration
{
    public static class RenderHtmlBuilder
    {
        /// <summary>
        /// Setup RenderHtmltoString library
        /// </summary>
        /// <param name="services">The IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddRenderHtmlToString(
            this IServiceCollection services
            )
        {
            AddCore(services, new UpdateableFileProvider());
            return services;
        }

        private static IServiceCollection AddCore(IServiceCollection services, UpdateableFileProvider fileProvider)
        {
            services.TryAddSingleton(fileProvider);
            services.TryAddSingleton<IRazorViewEngine, RazorViewEngine>();
            services.TryAddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            return services;
        }
    }
}
