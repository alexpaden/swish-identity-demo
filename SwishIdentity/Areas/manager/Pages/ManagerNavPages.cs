using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SwishIdentity.Areas.Manager.Pages
{
    public static class ManagerNavPages
    {
        public static string Share => "Share";

        public static string Index => "Index";
        
        public static string Request => "Request";
        
        
        public static string HomeNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        
        public static string ShareNavClass(ViewContext viewContext) => PageNavClass(viewContext, Share);
       
        public static string RequestNavClass(ViewContext viewContext) => PageNavClass(viewContext, Request);


        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
