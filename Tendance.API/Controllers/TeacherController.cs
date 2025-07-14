using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Teacher;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/teachers")]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}")]
    [ApiController]
    public class TeacherController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTeachersAsync()
        {
            List<TeacherForClient> teachers = await dbContext.Teachers
                .Include(teacher => teacher.School)
                .Where(teacher => teacher.School!.Id == userContext.SchoolId)
                .Select(teacher => new TeacherForClient
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    MiddleName = teacher.MiddleName,
                    Email = teacher.Email,
                })
                .ToListAsync();

            return Ok(teachers);
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateTeachersAsync([FromBody] TeacherForCreation teacherForCreation)
        {
            TeacherEntity teacher = new TeacherEntity
            {
                SchoolId = userContext.SchoolId,
                FirstName = teacherForCreation.FirstName,
                LastName = teacherForCreation.LastName,
                MiddleName = teacherForCreation.Middlename,
                Email = teacherForCreation.Email,
                Created = DateTime.UtcNow,
            };

            await dbContext.Teachers.AddAsync(teacher);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete("{teacherId:int}")]
        public async Task<IActionResult> DeleteTeacher([FromRoute] int teacherId)
        {
            TeacherEntity? teacher = await dbContext.Teachers.FirstOrDefaultAsync(teacher => teacher.Id == teacherId && teacher.SchoolId == userContext.SchoolId);
            if (teacher != null)
            {
                dbContext.Teachers.Remove(teacher);
                await dbContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
