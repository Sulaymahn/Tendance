using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Student;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/students")]
    [Authorize]
    [ApiController]
    public class StudentController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetStudent([FromHeader(Name = "X-Minimal")] bool? minimal)
        {
            IQueryable<Student> students = dbContext.Students
                .Where(student => student.SchoolId == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await students.Select(student => new StudentForClientMinimal
                {
                    Id = student.SchoolAssignedId,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    MiddleName = student.MiddleName,
                })
                .ToListAsync());
            }
            else
            {
                return Ok(await students.Select(student => new StudentForClient
                {
                    Id = student.SchoolAssignedId,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    MiddleName = student.MiddleName,
                    Email = student.Email,
                    Created = student.Created,
                    AttendanceRate = student.Classrooms.Count == 0 ? 0 : (float)student.Attendances.Count() / (float)student.Classrooms.Count(),
                })
                .ToListAsync());
            }
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentForCreation studentForCreation)
        {
            if (await dbContext.Students.AnyAsync(course => course.SchoolId == userContext.SchoolId && course.SchoolAssignedId == studentForCreation.Id.Trim()))
            {
                return Conflict("Already exists");
            }

            Student student = new Student
            {
                SchoolId = userContext.SchoolId,
                SchoolAssignedId = studentForCreation.Id.Trim(),
                Created = DateTime.UtcNow,
                Email = studentForCreation.Email,
                FirstName = studentForCreation.FirstName,
                LastName = studentForCreation.LastName,
                MiddleName = studentForCreation.MiddleName
            };

            dbContext.Students.Add(student);
            dbContext.SaveChanges();

            return Ok();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete]
        public async Task<IActionResult> DeleteStudent([FromHeader(Name = "X-Student-Id")] string studentId)
        {
            Student? student = await dbContext.Students.FirstOrDefaultAsync(course => course.SchoolAssignedId == studentId && course.SchoolId == userContext.SchoolId);
            if (student != null)
            {
                dbContext.Students.Remove(student);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
