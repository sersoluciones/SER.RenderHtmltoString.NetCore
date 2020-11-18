using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace SER.RenderHtmltoString.NetCore.Render.Models
{
    public class TemplateBindingModel : PageModel
    {
        public object Data { get; set; }

        public string Lang { get; set; }

        public string[] CSSPaths { get; set; }

        public List<string> Permissions { get; set; }
    }
}
