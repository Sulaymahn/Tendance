using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Student;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/students")]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}")]
    [ApiController]
    public class StudentController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetStudent()
        {
            List<StudentForClient> students = await dbContext.Students
                .Where(student => student.SchoolId == userContext.SchoolId)
                .Select(student => new StudentForClient
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    MiddleName = student.MiddleName,
                    Email = student.Email,
                    Created = student.Created,
                })
            .ToListAsync();

            return Ok(students);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStudentById([FromRoute] int id)
        {
            StudentEntity? student = await dbContext.Students
                .FirstOrDefaultAsync(student => student.SchoolId == userContext.SchoolId && student.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(new StudentForClient
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                MiddleName = student.MiddleName,
                Email = student.Email,
                Created = student.Created,
            });
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentForCreation studentForCreation)
        {
            StudentEntity student = new StudentEntity
            {
                SchoolId = userContext.SchoolId,
                Created = DateTime.UtcNow,
                Email = studentForCreation.Email,
                FirstName = studentForCreation.FirstName,
                LastName = studentForCreation.LastName,
                MiddleName = studentForCreation.MiddleName
            };

            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete("{studentId:int}")]
        public async Task<IActionResult> DeleteStudent([FromRoute] int studentId)
        {
            StudentEntity? student = await dbContext.Students.FirstOrDefaultAsync(student => student.Id == studentId && student.SchoolId == userContext.SchoolId);
            if (student != null)
            {
                dbContext.Students.Remove(student);
                await dbContext.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
