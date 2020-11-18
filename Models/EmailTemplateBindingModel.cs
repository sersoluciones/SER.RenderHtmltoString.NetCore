using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SER.RenderHtmltoString.NetCore.Render.Models
{
    public class EmailTemplateBindingModel : PageModel
    {
        public string FirstName { get; set; }

        public string Message { get; set; }

        public string Href { get; set; }
    }
}
