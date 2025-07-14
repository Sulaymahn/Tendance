using NodaTime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Classroom;
using Tendance.API.DataTransferObjects.ClassroomSession;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/sessions")]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}")]
    [ApiController]
    public class ClassroomSessionController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetClassroomSession()
        {
            List<ClassroomSessionForClient> sessions = await dbContext.ClassroomSessions
                .AsNoTracking()
                .Include(s => s.Classroom)
                .Where(session => session.Classroom != null && session.Classroom.SchoolId == userContext.SchoolId)
                .Select(session => new ClassroomSessionForClient
                {
                    Id = session.Id,
                    CheckInFrom = session.CheckInFrom,
                    CheckInTo = session.CheckInTo,
                    CheckOutFrom = session.CheckOutFrom,
                    CheckOutTo = session.CheckOutTo,
                    From = session.From,
                    Note = session.Note,
                    To = session.To,
                    Topic = session.Topic,
                    Created = session.Created,
                    Attendances = dbContext.StudentAttendances
                    .AsNoTracking()
                    .Include(att => att.Student)
                    .Where(att => att.ClassroomSessionId == session.Id)
                    .Select(att => new ClassroomSessionAttendance
                    {
                        CheckedIn = att.Type == AttendanceType.CheckIn,
                        CheckedOut = att.Type == AttendanceType.CheckOut,
                        FirstName = att.Student!.FirstName,
                        CheckInTimeStamp = att.Timestamp,
                        CheckOutTimeStamp = att.Timestamp,
                        LastName = att.Student!.LastName,
                        MiddleName = att.Student!.MiddleName,
                        Role = AttendanceRole.Student,
                        UserId = att.StudentId,
                    }).ToList(),
                    Classroom = new ClassroomForClient
                    {
                        Id = session.Classroom!.Id,
                        Course = new ClassroomCourse
                        {
                            Id = session.Classroom.Course!.Id,
                            Name = session.Classroom.Course.Name,
                        },
                        Room = new ClassroomRoom
                        {
                            Id = session.Classroom.Room!.Id,
                            Name = session.Classroom.Room.Name,
                            Building = session.Classroom.Room.Building,
                        },
                        Teacher = new ClassroomTeacher
                        {
                            Id = session.Classroom.Teacher!.Id,
                            FirstName = session.Classroom.Teacher.FirstName,
                            LastName = session.Classroom.Teacher.LastName,
                            MiddleName = session.Classroom.Teacher.MiddleName,
                        },
                        Students = session.Classroom.Students.Select(student => new ClassroomStudent
                        {
                            Id = student.Id,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            MiddleName = student.MiddleName,
                        })
                    },
                }).ToListAsync();

            return Ok(sessions);
        }

        [HttpGet("{sessionId:int}")]
        public async Task<IActionResult> GetClassroomSessionById([FromRoute] int sessionId)
        {
            ClassroomSessionForClient? session = await dbContext.ClassroomSessions
                .AsNoTracking()
                .Include(s => s.Classroom)
                .ThenInclude(c => c.Students)
                .Where(session => session.Classroom != null && session.Classroom.SchoolId == userContext.SchoolId && session.Id == sessionId)
                .Select(session => new ClassroomSessionForClient
                {
                    Id = session.Id,
                    CheckInFrom = session.CheckInFrom,
                    CheckInTo = session.CheckInTo,
                    CheckOutFrom = session.CheckOutFrom,
                    CheckOutTo = session.CheckOutTo,
                    From = session.From,
                    Note = session.Note,
                    To = session.To,
                    Topic = session.Topic,
                    Attendances = dbContext.StudentAttendances
                    .AsNoTracking()
                    .Include(att => att.Student)
                    .Where(att => att.ClassroomSessionId == session.Id)
                    .Select(att => new ClassroomSessionAttendance
                    {
                        CheckedIn = att.Type == AttendanceType.CheckIn,
                        CheckedOut = att.Type == AttendanceType.CheckOut,
                        FirstName = att.Student!.FirstName,
                        CheckInTimeStamp = att.Timestamp,
                        CheckOutTimeStamp = att.Timestamp,
                        LastName = att.Student!.LastName,
                        MiddleName = att.Student!.MiddleName,
                        Role = AttendanceRole.Student,
                        UserId = att.StudentId,
                    }).ToList(),
                    Classroom = new ClassroomForClient
                    {
                        Id = session.Classroom!.Id,
                        Course = new ClassroomCourse
                        {
                            Id = session.Classroom.Course!.Id,
                            Name = session.Classroom.Course.Name,
                        },
                        Room = new ClassroomRoom
                        {
                            Id = session.Classroom.Room!.Id,
                            Name = session.Classroom.Room.Name,
                            Building = session.Classroom.Room.Building,
                        },
                        Teacher = new ClassroomTeacher
                        {
                            Id = session.Classroom.Teacher!.Id,
                            FirstName = session.Classroom.Teacher.FirstName,
                            LastName = session.Classroom.Teacher.LastName,
                            MiddleName = session.Classroom.Teacher.MiddleName,
                        },
                        Students = session.Classroom.Students.Select(student => new ClassroomStudent
                        {
                            Id = student.Id,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            MiddleName = student.MiddleName,
                        })
                    },
                }).FirstOrDefaultAsync();

            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateClassroomSession([FromBody] ClassroomSessionForCreation sessionForCreation)
        {
            ClassroomEntity? classroom = await dbContext.Classrooms.FirstOrDefaultAsync(classroom => classroom.SchoolId == userContext.SchoolId && classroom.Id == sessionForCreation.ClassroomId);
            if (classroom == null)
            {
                return BadRequest("Classroom not found");
            }

            DateTimeZone? timezone = DateTimeZoneProviders.Tzdb[sessionForCreation.Timezone];
            if (timezone == null)
            {
                return BadRequest("Invalid Tzdb Timezone");
            }

            sessionForCreation.From = DateTime.SpecifyKind(sessionForCreation.From, DateTimeKind.Local);
            sessionForCreation.From = timezone.AtStrictly(LocalDateTime.FromDateTime(sessionForCreation.From)).ToDateTimeUtc();

            sessionForCreation.To = DateTime.SpecifyKind(sessionForCreation.To, DateTimeKind.Local);
            sessionForCreation.To = timezone.AtStrictly(LocalDateTime.FromDateTime(sessionForCreation.To)).ToDateTimeUtc();

            sessionForCreation.CheckInFrom = DateTime.SpecifyKind(sessionForCreation.CheckInFrom, DateTimeKind.Local);
            sessionForCreation.CheckInFrom = timezone.AtStrictly(LocalDateTime.FromDateTime(sessionForCreation.CheckInFrom)).ToDateTimeUtc();

            sessionForCreation.CheckInTo = DateTime.SpecifyKind(sessionForCreation.CheckInTo, DateTimeKind.Local);
            sessionForCreation.CheckInTo = timezone.AtStrictly(LocalDateTime.FromDateTime(sessionForCreation.CheckInTo)).ToDateTimeUtc();

            sessionForCreation.CheckOutFrom = DateTime.SpecifyKind(sessionForCreation.CheckOutFrom, DateTimeKind.Local);
            sessionForCreation.CheckOutFrom = timezone.AtStrictly(LocalDateTime.FromDateTime(sessionForCreation.CheckOutFrom)).ToDateTimeUtc();

            sessionForCreation.CheckOutTo = DateTime.SpecifyKind(sessionForCreation.CheckOutTo, DateTimeKind.Local);
            sessionForCreation.CheckOutTo = timezone.AtStrictly(LocalDateTime.FromDateTime(sessionForCreation.CheckOutTo)).ToDateTimeUtc();

            ClassroomSessionEntity session = new ClassroomSessionEntity
            {
                ClassroomId = classroom.Id,
                CheckInFrom = sessionForCreation.CheckInFrom,
                CheckInTo = sessionForCreation.CheckInTo,
                CheckOutFrom = sessionForCreation.CheckOutFrom,
                CheckOutTo = sessionForCreation.CheckOutTo,
                Created = DateTime.UtcNow,
                From = sessionForCreation.From,
                To = sessionForCreation.To,
                Topic = sessionForCreation.Topic,
                Note = sessionForCreation.Note,
            };

            await dbContext.ClassroomSessions.AddAsync(session);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete]
        public async Task<IActionResult> DeleteClassroomSession([FromHeader(Name = "X-Classroom-Session-Id")] int sessionId)
        {
            ClassroomSessionEntity? session = await dbContext.ClassroomSessions.FirstOrDefaultAsync(session => session.Id == sessionId && session.Classroom != null && session.Classroom.SchoolId == userContext.SchoolId);
            if (session != null)
            {
                dbContext.ClassroomSessions.Remove(session);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
