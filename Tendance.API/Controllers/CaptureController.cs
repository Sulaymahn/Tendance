﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Authorize(AuthenticationSchemes = DeviceAuthDefaults.AuthenticationScheme, Policy = TendancePolicy.DeviceOnly)]
    [Route("api/capture")]
    [ApiController]
    public class CaptureController(ApplicationDbContext dbContext, FacialRecognitionService facialRecognitionService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> MatchAsync([FromForm(Name = "file")] IFormFile file)
        {
            if (Request.HttpContext.Items[DeviceAuthDefaults.ContextKey] is not CaptureDeviceEntity device)
            {
                return BadRequest("Unknown Device");
            }

            bool classroomExist = await dbContext.Classrooms
                .AsNoTracking()
                .AnyAsync(classroom => classroom.SchoolId == device.SchoolId && classroom.Id == device.ClassroomId);

            if (!classroomExist)
            {
                return BadRequest("Classroom not set");
            }

            var now = DateTime.UtcNow;

            ClassroomSessionEntity? session = await dbContext.ClassroomSessions
                .AsNoTracking()
                .Where(session => session.ClassroomId == device.ClassroomId && ((session.CheckInFrom < now && session.CheckInTo > now) || (session.CheckOutFrom < now && session.CheckOutTo > now)))
                .OrderByDescending(session => session.To)
                .FirstOrDefaultAsync();

            bool? isCheckIn = session?.CheckInFrom < now && session.CheckInTo > now;

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            CaptureMatchResult result = device.Type switch
            {
                CaptureDeviceType.FacialRecognition => await facialRecognitionService.MatchAsync(bytes, device),
                _ => throw new ArgumentException("Unrecognized capture device type")
            };

            if (!result.Success || !isCheckIn.HasValue || session == null)
            {
                return Ok(result);
            }

            if (result.Role == AttendanceRole.Student)
            {
                await dbContext.StudentAttendances.AddAsync(new StudentAttendanceEntity
                {
                    SchoolId = device.SchoolId,
                    ClassroomSessionId = session!.Id,
                    StudentId = result.MatchId!.Value,
                    Timestamp = DateTime.UtcNow,
                    Type = isCheckIn.Value ? AttendanceType.CheckIn : AttendanceType.CheckOut,
                });

            }
            else if (result.Role == AttendanceRole.Teacher)
            {
                await dbContext.TeacherAttendances.AddAsync(new TeacherAttendanceEntity
                {
                    SchoolId = device.SchoolId,
                    ClassroomSessionId = session!.Id,
                    TeacherId = result.MatchId!.Value,
                    Timestamp = DateTime.UtcNow,
                    Type = isCheckIn.Value ? AttendanceType.CheckIn : AttendanceType.CheckOut,
                });
            }

            await dbContext.SaveChangesAsync();
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(
            [FromForm(Name = "UserRole")] AttendanceRole role,
            [FromForm(Name = "UserId")] int userId,
            [FromForm(Name = "File")] IFormFile file)
        {
            if (Request.HttpContext.Items[DeviceAuthDefaults.ContextKey] is not CaptureDeviceEntity device)
            {
                return BadRequest("Unknown Device");
            }

            object? person = role switch
            {
                AttendanceRole.Student => await dbContext.Students.FirstOrDefaultAsync(student => student.Id == userId && student.SchoolId == device.SchoolId),
                AttendanceRole.Teacher => await dbContext.Teachers.FirstOrDefaultAsync(teacher => teacher.Id == userId && teacher.SchoolId == device.SchoolId),
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
