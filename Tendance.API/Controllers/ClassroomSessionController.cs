using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Session;
using Tendance.API.Entities;
using Tendance.API.Services;

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
                .Where(session => session.Classroom != null && session.Classroom.SchoolId == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await sessions.Select(session => new ClassroomSessionForClient
                {
                    Id = session.Id,
                    ClassroomId = session.ClassroomId,
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
                    ClassroomId = session.ClassroomId,
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
