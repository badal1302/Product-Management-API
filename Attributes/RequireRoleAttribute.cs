using Microsoft.AspNetCore.Authorization;

namespace ProductManagementApi.Attributes
{
    /// <summary>
    /// Custom authorization attribute that requires specific roles
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }

    /// <summary>
    /// Predefined role attributes for common scenarios
    /// </summary>
    public class RequireAdminAttribute : RequireRoleAttribute
    {
        public RequireAdminAttribute() : base("Admin") { }
    }

    public class RequireUserOrAdminAttribute : RequireRoleAttribute
    {
        public RequireUserOrAdminAttribute() : base("User", "Admin") { }
    }
}
