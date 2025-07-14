using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Attendance;
using Tendance.API.Entities;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}")]
    [Route("api/attendances")]
    [ApiController]
    public class AttendanceController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAttendaces()
        {
            IQueryable<AttendanceForClient> students = dbContext.StudentAttendances
                .AsNoTracking()
                .Include(att => att.ClassroomSession)
                .Where(attendance => attendance.SchoolId == userContext.SchoolId)
                .Select(attendance => new AttendanceForClient
                {
                    FirstName = attendance.Student!.FirstName,
                    MiddleName = attendance.Student.MiddleName,
                    LastName = attendance.Student.LastName,
                    Timestamp = attendance.Timestamp,
                    Role = AttendanceRole.Student,
                    Type = attendance.Type,
                    UserId = attendance.StudentId,
                    Classroom = new AttendanceClassroom
                    {
                        Id = attendance.ClassroomSession!.ClassroomId,
                        ClassroomSessionId = attendance.ClassroomSessionId,
                    }
                });

            IQueryable<AttendanceForClient> teachers = dbContext.TeacherAttendances
                .AsNoTracking()
                .Include(att => att.ClassroomSession)
                .Where(attendance => attendance.SchoolId == userContext.SchoolId)
                .Select(attendance => new AttendanceForClient
                {
                    FirstName = attendance.Teacher!.FirstName,
                    MiddleName = attendance.Teacher.MiddleName,
                    LastName = attendance.Teacher.LastName,
                    Timestamp = attendance.Timestamp,
                    Role = AttendanceRole.Student,
                    Type = attendance.Type,
                    UserId = attendance.TeacherId,
                    Classroom = new AttendanceClassroom
                    {
                        Id = attendance.ClassroomSession!.ClassroomId,
                        ClassroomSessionId = attendance.ClassroomSessionId,
                    }
                });

            List<AttendanceForClient> attendances = await students
                .Concat(teachers)
                .OrderByDescending(att => att.Timestamp)
                .ToListAsync();

            return Ok(attendances);
        }
    }
}
