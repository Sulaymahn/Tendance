using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Auth;
using Tendance.API.Entities;
using Tendance.API.Models;

namespace Tendance.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly JsonWebTokenHandler _tokenHandler;
        private readonly SigningCredentials _signingCredentials;
        private readonly TokenValidationParameters _tokenValidationParameters;

        private readonly ApplicationDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationController(
            ApplicationDbContext dbContext,
            JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
            _dbContext = dbContext;
            _tokenHandler = new JsonWebTokenHandler();
            var securitykey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("TENDANCE_API_AUTH_TOKEN_KEY") ?? throw new ArgumentException("TOKEN KEY NOT FOUND")));
            _signingCredentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = securitykey,
                ValidateLifetime = true
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginCredential credential)
        {
            credential.Email = credential.Email.ToLower();
            User? user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == credential.Email && user.Password == user.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = new RefreshToken
            {
                IsConsumed = false,
                Created = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow + TimeSpan.FromMinutes(_jwtSettings.RefreshTokenExpiry),
                UserId = user.Id,
                Token = GenerateRandomToken(32, true)
            };

            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new("UserId", user.Id.ToString()),
                new("SchoolId", user.SchoolId.ToString())
            };

            return Ok(new AuthToken
            {
                AccessToken = GenerateJwtToken(claims),
                RefreshToken = token.Token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> SignupAsync([FromBody] SignupCredential credential)
        {
            credential.Email = credential.Email.ToLower();
            credential.SchoolEmail = credential.SchoolEmail.ToLower();

            User? user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == credential.Email);

            School? school = await _dbContext.Schools
                .AsNoTracking()
                .FirstOrDefaultAsync(school => school.Email == credential.SchoolEmail);

            if (user != null || school != null)
            {
                return Unauthorized();
            }

            school = new School
            {
                Id = Guid.NewGuid(),
                Email = credential.SchoolEmail.ToLower(),
                Joined = DateTime.UtcNow,
                Name = credential.SchoolName
            };

            await _dbContext.Schools.AddAsync(school);

            user = new User
            {
                Id = Guid.NewGuid(),
                SchoolId = school.Id,
                Created = DateTime.UtcNow,
                Email = credential.Email.ToLower(),
                Username = credential.Username,
                Password = credential.Password,
                School = school
            };

            await _dbContext.Users.AddAsync(user);

            var token = new RefreshToken
            {
                IsConsumed = false,
                Created = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow + TimeSpan.FromMinutes(_jwtSettings.RefreshTokenExpiry),
                UserId = user.Id,
                Token = GenerateRandomToken(32, true)
            };

            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new("UserId", user.Id.ToString()),
                new("SchoolId", user.SchoolId.ToString())
            };

            return Ok(new AuthToken
            {
                AccessToken = GenerateJwtToken(claims),
                RefreshToken = token.Token
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request)
        {
            RefreshToken? oldRefreshToken = await _dbContext.RefreshTokens
                .Include(refresh => refresh.User)
                .FirstOrDefaultAsync(refresh => refresh.Token == request.RefreshToken && !refresh.IsConsumed && DateTime.UtcNow < refresh.ExpiresAt);

            if (oldRefreshToken == null)
            {
                return Unauthorized();
            }

            oldRefreshToken.IsConsumed = true;

            var token = new RefreshToken
            {
                IsConsumed = false,
                Created = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow + TimeSpan.FromMinutes(_jwtSettings.RefreshTokenExpiry),
                UserId = oldRefreshToken.User!.Id,
                Token = GenerateRandomToken(32, true)
            };

            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new("UserId", oldRefreshToken.User.Id.ToString()),
                new("SchoolId", oldRefreshToken.User.SchoolId.ToString())
            };

            return Ok(new AuthToken
            {
                AccessToken = GenerateJwtToken(claims),
                RefreshToken = token.Token
            });
        }

        private string GenerateJwtToken(List<Claim> claims)
        {
            var token = new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow + TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpiry),
                SigningCredentials = _signingCredentials
            };

            return _tokenHandler.CreateToken(token);
        }

        private static string GenerateRandomToken(int length, bool urlSafe)
        {
            byte[] bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var token = Convert.ToBase64String(bytes);

            if (!urlSafe)
            {
                return token;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < token.Length; i++)
            {
                switch (token[i])
                {
                    case '+':
                        sb.Append('-');
                        break;
                    case '/':
                        sb.Append('_');
                        break;
                    case '=':
                        continue;
                    default:
                        sb.Append(token[i]);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}
