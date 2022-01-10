using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SwishIdentity.Areas.Verif.Pages
{
    public static class VerifNavPages
    {
        public static string Index => "Index";
        public static string Exists => "Exists";

        public static string HomeNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string ExistsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Exists);

        
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
