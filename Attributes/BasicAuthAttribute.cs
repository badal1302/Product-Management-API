using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductManagementApi.Services;
using System.Text;

namespace ProductManagementApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if Authorization header exists
            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            
            // Check if it's Basic authentication
            if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                // Extract username and password from Basic auth header
                var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                
                var separatorIndex = credentials.IndexOf(':');
                if (separatorIndex == -1)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var username = credentials.Substring(0, separatorIndex);
                var password = credentials.Substring(separatorIndex + 1);

                // Validate credentials
                var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                if (!authService.ValidateCredentials(username, password))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // Store user info for use in controller
                var user = authService.GetUser(username);
                if (user != null)
                {
                    context.HttpContext.Items["User"] = user;
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
