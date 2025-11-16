using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PROG6212_PART_3.Helpers
{
    // Custom Authorization Attribute for Session-based Auth
    public class SessionAuthorize : ActionFilterAttribute
    {
        public string[] Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            var userRole = context.HttpContext.Session.GetString("Role");

            // Check if user is logged in
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            // Check if user has required role
            if (Roles != null && Roles.Length > 0)
            {
                if (!Roles.Contains(userRole))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Login", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }

    // Extension methods for Session
    public static class SessionExtensions
    {
        public static int? GetUserId(this ISession session)
        {
            var userId = session.GetString("UserId");
            return string.IsNullOrEmpty(userId) ? null : int.Parse(userId);
        }

        public static string GetUserRole(this ISession session)
        {
            return session.GetString("Role") ?? "";
        }

        public static string GetFullName(this ISession session)
        {
            return session.GetString("FullName") ?? "";
        }

        public static bool IsLoggedIn(this ISession session)
        {
            return !string.IsNullOrEmpty(session.GetString("UserId"));
        }

        public static bool IsInRole(this ISession session, string role)
        {
            return session.GetString("Role") == role;
        }
    }
}
