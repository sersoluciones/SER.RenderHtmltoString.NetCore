using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SER.RenderHtmltoString.NetCore.Interfaces
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
        Task<string> RenderHtmlToStringAsync<TModel>(string html, TModel model);
        Task<string> RenderViewToStringAsync(string viewName);
        Task<string> RenderHtmlToStringAsync(string html);
        void UpdateView(string path, string viewHtml);
        bool ExistsView(string path);
        void AddView(string path, string viewHtml);
    }
}
