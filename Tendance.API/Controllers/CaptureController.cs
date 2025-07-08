using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/capture")]
    [ApiController]
    public class CaptureController(ApplicationDbContext dbContext, FacialRecognitionService facialRecognitionService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CaptureAsync(
            [FromHeader(Name = "X-Client-Key")] string clientKey,
            [FromHeader(Name = "X-Attendance-Type")] AttendanceType attendanceType,
            [FromForm(Name = "file")] IFormFile file)
        {
            CaptureDevice? device = await dbContext.Devices.FirstOrDefaultAsync(device => device.ClientKey == clientKey);
            if (device == null)
            {
                return BadRequest("Unknown Device");
            }

            if (device.ClassroomId == null)
            {
                return BadRequest("Classroom not set");
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            CaptureMatchResult result = device.Type switch
            {
                CaptureDeviceType.FacialRecognition => await facialRecognitionService.MatchAsync(bytes, device),
                _ => throw new ArgumentException("Unrecognized capture device type")
            };

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(
            [FromHeader(Name = "X-Client-Key")] string clientKey,
            [FromHeader(Name = "X-Capture-Role")] AttendanceRole role,
            [FromHeader(Name = "X-Capture-Owner-Id")] string ownerId,
            [FromForm(Name = "file")] IFormFile file)
        {
            CaptureDevice? device = await dbContext.Devices.FirstOrDefaultAsync(device => device.ClientKey == clientKey);
            if (device == null)
            {
                return BadRequest("Unknown Device");
            }

            object? person = role switch
            {
                AttendanceRole.Student => await dbContext.Students.FirstOrDefaultAsync(student => student.SchoolAssignedId == ownerId && student.SchoolId == device.SchoolId),
                AttendanceRole.Teacher => await dbContext.Teachers.FirstOrDefaultAsync(teacher => teacher.Id == int.Parse(ownerId) && teacher.SchoolId == device.SchoolId),
                _ => throw new ArgumentException("Unknown role")
            };

            if (person == null)
            {
                return BadRequest("Could not find owner");
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            CaptureRegisterResult result = device.Type switch
            {
                CaptureDeviceType.FacialRecognition => await facialRecognitionService.RegisterAsync(bytes, device, role, person),
                _ => throw new ArgumentException("Unrecognized capture device type")
            };

            return Ok();
        }
    }
}
