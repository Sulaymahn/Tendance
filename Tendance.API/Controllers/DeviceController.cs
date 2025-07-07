using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Classroom;
using Tendance.API.DataTransferObjects.Course;
using Tendance.API.DataTransferObjects.Device;
using Tendance.API.DataTransferObjects.Room;
using Tendance.API.DataTransferObjects.Teacher;
using Tendance.API.Entities;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/devices")]
    [Authorize]
    [ApiController]
    public class DeviceController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet("types")]
        public ActionResult<string[]> GetDeviceTypesAsync()
        {
            return Ok(Enum.GetNames<CaptureDeviceType>());
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices([FromHeader(Name = "X-Minimal")] bool? minimal)
        {
            IQueryable<CaptureDevice> devices = dbContext.Devices
                .AsNoTracking()
                .Where(course => course.SchoolId == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await devices.Select(device => new CaptureDeviceForClientMinimal
                {
                    Id = device.Id,
                    Created = device.Created,
                    Nickname = device.Nickname,
                    Type = device.Type,
                    Classroom = device.Classroom != null ? new ClassroomForClientMinimal
                    {
                        Id = device.Classroom.Id,
                        Course = new CourseForClientMinimal
                        {
                            Id = device.Classroom.Course!.Id,
                            Name = device.Classroom.Course.Name,
                        },
                        Room = new RoomForClientMinimal
                        {
                            Id = device.Classroom.Room!.Id,
                            Building = device.Classroom.Room.Building,
                            Name = device.Classroom.Room.Name,
                        },
                        Teacher = new TeacherForClientMinimal
                        {
                            Id = device.Classroom.Teacher!.Id,
                            FirstName = device.Classroom.Teacher.FirstName,
                            LastName = device.Classroom.Teacher.LastName,
                            MiddleName = device.Classroom.Teacher.MiddleName,
                        }
                    } : null,
                }).ToListAsync());
            }
            else
            {
                return Ok(await devices.Select(device => new CaptureDeviceForClient
                {
                    Id = device.Id,
                    Created = device.Created,
                    ClientKey = device.ClientKey,
                    Nickname = device.Nickname,
                    Type = device.Type,
                    Classroom = device.Classroom != null ? new ClassroomForClientMinimal
                    {
                        Id = device.Classroom.Id,
                        Course = new CourseForClientMinimal
                        {
                            Id = device.Classroom.Course!.Id,
                            Name = device.Classroom.Course.Name,
                        },
                        Room = new RoomForClientMinimal
                        {
                            Id = device.Classroom.Room!.Id,
                            Building = device.Classroom.Room.Building,
                            Name = device.Classroom.Room.Name,
                        },
                        Teacher = new TeacherForClientMinimal
                        {
                            Id = device.Classroom.Teacher!.Id,
                            FirstName = device.Classroom.Teacher.FirstName,
                            LastName = device.Classroom.Teacher.LastName,
                            MiddleName = device.Classroom.Teacher.MiddleName,
                        }
                    } : null,
                }).ToListAsync());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody] CaptureDeviceForCreation dto)
        {
            if (dto.ClassroomId == null)
            {
                CaptureDevice device = new CaptureDevice
                {
                    Id = Guid.NewGuid(),
                    SchoolId = userContext.SchoolId,
                    Created = DateTime.UtcNow,
                    ClientKey = $"dev-{Guid.NewGuid()}",
                    Type = dto.Type,
                    Nickname = dto.Nickname,
                };

                await dbContext.Devices.AddAsync(device);
            }
            else
            {
                Classroom? classroom = await dbContext.Classrooms.FirstOrDefaultAsync(classroom => classroom.SchoolId == userContext.SchoolId && classroom.Id == dto.ClassroomId);
                if (classroom == null)
                {
                    return BadRequest("Classrooom");
                }

                CaptureDevice device = new CaptureDevice
                {
                    Id = Guid.NewGuid(),
                    SchoolId = userContext.SchoolId,
                    Created = DateTime.UtcNow,
                    ClientKey = $"dev-{Guid.NewGuid()}",
                    Type = dto.Type,
                    Nickname = dto.Nickname,
                    ClassroomId = classroom.Id,
                };

                await dbContext.Devices.AddAsync(device);
            }

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDevice([FromHeader(Name = "X-Device-Id")] Guid deviceId)
        {
            CaptureDevice? device = await dbContext.Devices.FirstOrDefaultAsync(device => device.Id == deviceId && device.SchoolId == userContext.SchoolId);
            if (device != null)
            {
                dbContext.Devices.Remove(device);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
