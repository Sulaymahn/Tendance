using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Tendance.API.Data;
using Tendance.API.Entities;
using Tendance.API.Models;

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
            if (!header.StartsWith($"{DeviceAuthDefaults.AuthenticationScheme} ")) return AuthenticateResult.Fail("Invalid scheme");

            string clientKey = header[$"{DeviceAuthDefaults.AuthenticationScheme} ".Length..];

            CaptureDeviceEntity? device = await dbContext.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(device => device.ClientKey == clientKey);

            if (device == null)
                return AuthenticateResult.Fail("Invalid device");

            var claims = new List<Claim> {
                new(TendanceClaim.DeviceId, device.Id.ToString()),
                new(TendanceClaim.SchoolId, device.SchoolId.ToString()),
                new(TendanceClaim.DeviceType, device.Type.ToString()),
            };

            Request.HttpContext.Items[DeviceAuthDefaults.ContextKey] = device;

            var identity = new ClaimsIdentity(claims, nameof(DeviceAuthenticationHandler));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
