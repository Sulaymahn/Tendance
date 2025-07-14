using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.CaptureDevice;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/devices")]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}", Policy = TendancePolicy.UserOnly)]
    [ApiController]
    public class DeviceController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("types")]
        public ActionResult<string[]> GetDeviceTypesAsync()
        {
            return Ok(Enum.GetNames<CaptureDeviceType>());
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            List<CaptureDeviceForClient> devices = await dbContext.Devices
                .AsNoTracking()
                .Where(course => course.SchoolId == userContext.SchoolId)
                .Select(device => new CaptureDeviceForClient
                {
                    Id = device.Id,
                    Created = device.Created,
                    Nickname = device.Nickname,
                    Type = device.Type,
                    Mode = device.Mode,
                    ClassroomId = device.ClassroomId,
                    ClientKey = device.ClientKey,
                }).ToListAsync();

            return Ok(devices);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody] CaptureDeviceForCreation dto)
        {
            if (dto.ClassroomId == null)
            {
                CaptureDeviceEntity device = new CaptureDeviceEntity
                {
                    SchoolId = userContext.SchoolId,
                    Created = DateTime.UtcNow,
                    ClientKey = $"dev-{Guid.NewGuid()}",
                    Type = dto.Type,
                    Mode = CaptureDeviceMode.Idle,
                    Nickname = dto.Nickname,
                };

                await dbContext.Devices.AddAsync(device);
            }
            else
            {
                ClassroomEntity? classroom = await dbContext.Classrooms.FirstOrDefaultAsync(classroom => classroom.SchoolId == userContext.SchoolId && classroom.Id == dto.ClassroomId);
                if (classroom == null)
                {
                    return BadRequest("Classrooom");
                }

                CaptureDeviceEntity device = new CaptureDeviceEntity
                {
                    SchoolId = userContext.SchoolId,
                    Created = DateTime.UtcNow,
                    ClientKey = $"dev-{Guid.NewGuid()}",
                    Type = dto.Type,
                    Mode = CaptureDeviceMode.Idle,
                    Nickname = dto.Nickname,
                    ClassroomId = classroom.Id,
                };

                await dbContext.Devices.AddAsync(device);
            }

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{deviceId:int}")]
        public async Task<IActionResult> DeleteDevice([FromRoute] int deviceId)
        {
            CaptureDeviceEntity? device = await dbContext.Devices.FirstOrDefaultAsync(device => device.Id == deviceId && device.SchoolId == userContext.SchoolId);
            if (device != null)
            {
                dbContext.Devices.Remove(device);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
