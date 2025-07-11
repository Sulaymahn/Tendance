using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Classroom;
using Tendance.API.DataTransferObjects.ClassroomSession;
using Tendance.API.DataTransferObjects.Course;
using Tendance.API.DataTransferObjects.Room;
using Tendance.API.DataTransferObjects.Student;
using Tendance.API.DataTransferObjects.Teacher;
using Tendance.API.Entities;
using Tendance.API.Services;
using NodaTime;

namespace Tendance.API.Controllers
{
    [Route("api/sessions")]
    [Authorize]
    [ApiController]
    public class ClassroomSessionController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetClassroomSession([FromHeader(Name = "X-Minimal")] bool? minimal)
        {
            IQueryable<ClassroomSession> sessions = dbContext.ClassroomSessions
                .AsNoTracking()
                .Include(s => s.Classroom)
                .Where(session => session.Classroom != null && session.Classroom.SchoolId == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await sessions.Select(session => new ClassroomSessionForClient
                {
                    Id = session.Id,
                    Classroom = new ClassroomForClientMinimal
                    {
                        Id = session.Classroom!.Id,
                        Course = new CourseForClientMinimal
                        {
                            Id = session.Classroom.Course!.Id,
                            Name = session.Classroom.Course.Name,
                        },
                        Room = new RoomForClientMinimal
                        {
                            Id = session.Classroom.Room!.Id,
                            Name = session.Classroom.Room.Name,
                            Building = session.Classroom.Room.Building,
                        },
                        Teacher = new TeacherForClientMinimal
                        {
                            Id = session.Classroom.Teacher!.Id,
                            FirstName = session.Classroom.Teacher.FirstName,
                            LastName = session.Classroom.Teacher.LastName,
                            MiddleName = session.Classroom.Teacher.MiddleName,
                        },
                        Students = session.Classroom.Students.Select(student => new StudentForClientMinimal
                        {
                            Id = student.SchoolAssignedId,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            MiddleName = student.MiddleName,
                        })
                    },
                    CheckInFrom = session.CheckInFrom,
                    CheckInTo = session.CheckInTo,
                    CheckOutFrom = session.CheckOutFrom,
                    CheckOutTo = session.CheckOutTo,
                    From = session.From,
                    Note = session.Note,
                    To = session.To,
                    Topic = session.Topic,
                }).ToListAsync());
            }
            else
            {
                return Ok(await sessions.Select(session => new ClassroomSessionForClient
                {
                    Id = session.Id,
                    Classroom = new ClassroomForClientMinimal
                    {
                        Id = session.Classroom!.Id,
                        Course = new CourseForClientMinimal
                        {
                            Id = session.Classroom.Course!.Id,
                            Name = session.Classroom.Course.Name,
                        },
                        Room = new RoomForClientMinimal
                        {
                            Id = session.Classroom.Room!.Id,
                            Name = session.Classroom.Room.Name,
                            Building = session.Classroom.Room.Building,
                        },
                        Teacher = new TeacherForClientMinimal
                        {
                            Id = session.Classroom.Teacher!.Id,
                            FirstName = session.Classroom.Teacher.FirstName,
                            LastName = session.Classroom.Teacher.LastName,
                            MiddleName = session.Classroom.Teacher.MiddleName,
                        },
                        Students = session.Classroom.Students.Select(student => new StudentForClientMinimal
                        {
                            Id = student.SchoolAssignedId,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            MiddleName = student.MiddleName,
                        })
                    },
                    CheckInFrom = session.CheckInFrom,
                    CheckInTo = session.CheckInTo,
                    CheckOutFrom = session.CheckOutFrom,
                    CheckOutTo = session.CheckOutTo,
                    From = session.From,
                    Note = session.Note,
                    To = session.To,
                    Topic = session.Topic,
                }).ToListAsync());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateClassroomSession([FromBody] ClassroomSessionForCreation sessionForCreation)
        {
            Classroom? classroom = await dbContext.Classrooms.FirstOrDefaultAsync(classroom => classroom.SchoolId == userContext.SchoolId && classroom.Id == sessionForCreation.ClassroomId);
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

            ClassroomSession session = new ClassroomSession
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

        [HttpDelete]
        public async Task<IActionResult> DeleteClassroomSession([FromHeader(Name = "X-Classroom-Session-Id")] int sessionId)
        {
            ClassroomSession? session = await dbContext.ClassroomSessions.FirstOrDefaultAsync(session => session.Id == sessionId && session.Classroom != null && session.Classroom.SchoolId == userContext.SchoolId);
            if (session != null)
            {
                dbContext.ClassroomSessions.Remove(session);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
