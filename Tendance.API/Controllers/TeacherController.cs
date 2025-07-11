using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Teacher;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/teachers")]
    [Authorize]
    [ApiController]
    public class TeacherController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTeachersAsync([FromHeader(Name = "X-Minimal")] bool? minimal)
        {
            IQueryable<Teacher> teachers = dbContext.Teachers
                .Include(teacher => teacher.School)
                .Where(teacher => teacher.School!.Id == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await teachers.Select(teacher => new TeacherForClientMinimal
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    MiddleName = teacher.MiddleName,
                })
                .ToListAsync());
            }
            else
            {
                return Ok(await teachers.Select(teacher => new TeacherForClient
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    MiddleName = teacher.MiddleName,
                    Email = teacher.Email,
                    AttendanceRate = teacher.Classrooms.Count == 0 ? 0 : (float)teacher.Attendances.Count() / (float)teacher.Classrooms.Count(),
                    Created = teacher.Created
                })
                .ToListAsync());
            }
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateTeachersAsync([FromBody] TeacherForCreation teacherForCreation)
        {
            Teacher teacher = new Teacher
            {
                SchoolId = userContext.SchoolId,
                FirstName = teacherForCreation.FirstName,
                LastName = teacherForCreation.LastName,
                MiddleName = teacherForCreation.Middlename,
                Email = teacherForCreation.Email,
                Created = DateTime.UtcNow,
            };

            await dbContext.Teachers.AddAsync(teacher);
            dbContext.SaveChanges();

            return Ok();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete]
        public async Task<IActionResult> DeleteTeacher([FromHeader(Name = "X-Teacher-Id")] int teacherId)
        {
            Teacher? teacher = await dbContext.Teachers.FirstOrDefaultAsync(teacher => teacher.Id == teacherId && teacher.SchoolId == userContext.SchoolId);
            if (teacher != null)
            {
                dbContext.Teachers.Remove(teacher);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
