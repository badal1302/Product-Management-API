using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using ProductManagementApi.Services;

namespace ProductManagementApi.Auth
{
    public class DualAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public DualAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthService authService,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _authService = authService;
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader))
                return AuthenticateResult.NoResult();

            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // JWT validation
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var jwtSection = _configuration.GetSection("Jwt");
                var key = jwtSection["Key"];
                var issuer = jwtSection["Issuer"];
                var audience = jwtSection["Audience"];
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                    return AuthenticateResult.Fail("JWT config missing");
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    }, out var validatedToken);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                catch
                {
                    return AuthenticateResult.Fail("Invalid JWT token");
                }
            }
            else if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                // Basic Auth validation
                var encoded = authHeader.Substring("Basic ".Length).Trim();
                try
                {
                    var credentialBytes = Convert.FromBase64String(encoded);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                    if (credentials.Length == 2)
                    {
                        var username = credentials[0];
                        var password = credentials[1];
                        if (_authService.ValidateCredentials(username, password))
                        {
                            var user = _authService.GetUser(username);
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.Name, username),
                                new Claim(ClaimTypes.Role, user?.Role ?? "User")
                            };
                            var identity = new ClaimsIdentity(claims, Scheme.Name);
                            var principal = new ClaimsPrincipal(identity);
                            var ticket = new AuthenticationTicket(principal, Scheme.Name);
                            return AuthenticateResult.Success(ticket);
                        }
                    }
                }
                catch
                {
                    return AuthenticateResult.Fail("Invalid Basic Auth header");
                }
                return AuthenticateResult.Fail("Invalid username or password");
            }
            return AuthenticateResult.NoResult();
        }
    }
}
