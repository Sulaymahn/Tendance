using Microsoft.AspNetCore.Authentication;

namespace Tendance.API.Authentication
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddTendanceDevice(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, DeviceAuthenticationHandler>(DeviceAuthDefaults.AuthenticationScheme, null);
        }
    }
}
