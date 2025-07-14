using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.User;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/user")]
    [Authorize(Policy = TendancePolicy.UserOnly)]
    [ApiController]
    public class UserController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUserAsync()
        {
            UserEntity? user = await dbContext.Users.FirstOrDefaultAsync(user => userContext.UserId == user.Id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserForClient
            {
                Email = user.Email,
                Username = user.Username
            });
        }
    }
}
