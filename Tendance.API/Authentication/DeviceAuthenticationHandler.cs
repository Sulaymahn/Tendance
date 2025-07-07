using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Tendance.API.Data;

namespace Tendance.API.Authentication
{
    public class DeviceAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder,
            ApplicationDbContext dbContext
            ) : AuthenticationHandler<AuthenticationSchemeOptions>(logger: loggerFactory, encoder: urlEncoder, options: options)
    {
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out StringValues value))
                return AuthenticateResult.Fail("Missing Authorization Header");

            string header = value.ToString();
            if (!header.StartsWith("Device ")) return AuthenticateResult.Fail("Invalid scheme");

            string clientKey = header["Device ".Length..];
            var device = await dbContext.Devices.FirstOrDefaultAsync(device => device.ClientKey == clientKey);
            if (device == null)
                return AuthenticateResult.Fail("Invalid device");

            var claims = new List<Claim> {
                new("DeviceId", device.Id.ToString()),
                new("SchoolId", device.SchoolId.ToString()),
                new("DeviceType", device.Type.ToString()),
            };

            var identity = new ClaimsIdentity(claims, nameof(DeviceAuthenticationHandler));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
