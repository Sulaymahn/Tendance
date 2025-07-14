using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Room;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}")]
    [Route("api/rooms")]
    [ApiController]
    public class RoomController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            List<RoomForClient> rooms = await dbContext.Rooms
                .Where(room => room.SchoolId == userContext.SchoolId)
                .Select(room => new RoomForClient
                {
                    Id = room.Id,
                    Name = room.Name,
                    Building = room.Building
                }).ToListAsync();

            return Ok(rooms);
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomForCreation dto)
        {
            var room = new RoomEntity
            {
                Name = dto.Name,
                Building = dto.Building,
                SchoolId = userContext.SchoolId
            };

            dbContext.Rooms.Add(room);
            await dbContext.SaveChangesAsync();

            return Created();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete("{roomId:int}")]
        public async Task<IActionResult> DeleteRoom([FromRoute] int roomId)
        {
            RoomEntity? room = await dbContext.Rooms.FirstOrDefaultAsync(room => room.Id == roomId && room.SchoolId == userContext.SchoolId);

            if (room == null)
            {
                return NotFound();
            }

            dbContext.Rooms.Remove(room);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
