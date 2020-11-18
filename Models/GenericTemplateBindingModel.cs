using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SER.RenderHtmltoString.NetCore.Render.Models
{
    public class GenericTemplateBindingModel<T> : PageModel where T : class
    {
        public T Data { get; set; }

        public string CssString { get; set; }
        public string[] Partials { get; set; }
    }
}
