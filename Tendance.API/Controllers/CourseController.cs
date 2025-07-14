using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Course;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}")]
    [Route("api/courses")]
    [ApiController]
    public class CourseController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCourse()
        {
            List<CourseForClient> courses = await dbContext.Courses
                .Where(course => course.SchoolId == userContext.SchoolId)
                .Select(course => new CourseForClient
                {
                    Id = course.Id,
                    Description = course.Description,
                    Name = course.Name,
                    Created = course.Created
                }).ToListAsync();

            return Ok(courses);
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseForCreation courseForCreation)
        {
            CourseEntity course = new CourseEntity
            {
                SchoolId = userContext.SchoolId,
                Description = courseForCreation.Description,
                Name = courseForCreation.Name,
                Created = DateTime.UtcNow,
            };

            await dbContext.Courses.AddAsync(course);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete]
        public async Task<IActionResult> DeleteCourse([FromHeader(Name = "X-Course-Id")] int courseId)
        {
            CourseEntity? course = await dbContext.Courses.FirstOrDefaultAsync(course => course.Id == courseId && course.SchoolId == userContext.SchoolId);
            if (course != null)
            {
                dbContext.Courses.Remove(course);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
