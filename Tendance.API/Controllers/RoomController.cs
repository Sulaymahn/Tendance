using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.Entities;
using Tendance.API.Services;
using Tendance.API.DataTransferObjects.Room;

namespace Tendance.API.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomForClient>>> GetRooms([FromHeader(Name = "X-Minimal")] bool? minimal)
        {
            IQueryable<Room> rooms = dbContext.Rooms
                .Where(r => r.SchoolId == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await rooms.Select(r => new RoomForClientMinimal
                {
                    Id = r.Id,
                    Name = r.Name,
                    Building = r.Building
                }).ToListAsync());
            }
            else
            {
                return Ok(await rooms.Select(r => new RoomForClient
                {
                    Id = r.Id,
                    Name = r.Name,
                    Building = r.Building
                }).ToListAsync());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomForCreation dto)
        {
            var room = new Room
            {
                Name = dto.Name,
                Building = dto.Building,
                SchoolId = userContext.SchoolId
            };

            dbContext.Rooms.Add(room);
            await dbContext.SaveChangesAsync();

            return Created();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoom([FromHeader(Name = "X-Room-Id")] int id)
        {
            var schoolId = userContext.SchoolId;
            var room = await dbContext.Rooms
                .FirstOrDefaultAsync(r => r.Id == id && r.SchoolId == schoolId);

            if (room == null)
                return NotFound();

            dbContext.Rooms.Remove(room);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
