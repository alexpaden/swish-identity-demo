using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SwishIdentity.Areas.Admin.Pages
{
    public static class AdminNavPages
    {
        public static string VerificationReview => "VerificationReview";
        
        public static string VerifHistory => "VerifHistory";
        
        public static string ReviewNavClass(ViewContext viewContext) => PageNavClass(viewContext, VerificationReview);

        public static string HistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, VerifHistory);
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
