using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SER.RenderHtmltoString.NetCore.Interfaces
{
    public interface IRazorPartialToStringRenderer
    {
        Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model);
    }
}
