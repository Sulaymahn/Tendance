using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddControllers()
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<UserContextAccessor>();
            builder.Services.AddScoped<FacialRecognitionService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDbContext<ApplicationDbContext>();

            JwtSettings jwtSetting = builder.Configuration.GetRequiredSection("JwtSettings").Get<JwtSettings>()!;
            builder.Services.AddSingleton(jwtSetting);
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.MapInboundClaims = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSetting.Issuer,
                    ValidAudience = jwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TENDANCE_API_AUTH_TOKEN_KEY") ?? throw new ArgumentException("TOKEN KEY NOT FOUND"))),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(pol =>
                {
                    pol.AllowAnyOrigin();
                    pol.AllowAnyHeader();
                    pol.AllowAnyMethod();
                });
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            //app.UseHttpsRedirection();
            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
