using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SwishIdentity.Areas.Profile.Pages
{
    public static class ProfileNavPages
    {
        public static string Create => "Create";

        public static string Update => "Update";

        public static string Index => "Index";
        
        public static string History => "History";
        
        public static string HomeNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        
        public static string CreateNavClass(ViewContext viewContext) => PageNavClass(viewContext, Create);

        public static string UpdateNavClass(ViewContext viewContext) => PageNavClass(viewContext, Update);

        public static string HistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, History);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
