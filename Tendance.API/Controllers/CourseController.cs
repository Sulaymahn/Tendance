using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Course;
using Tendance.API.Entities;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Authorize]
    [Route("api/courses")]
    [ApiController]
    public class CourseController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCourse([FromHeader(Name = "X-Minimal")] bool? minimal)
        {
            IQueryable<Course> courses = dbContext.Courses
                .Where(course => course.SchoolId == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await courses.Select(course => new CourseForClientMinimal
                {
                    Id = course.Id,
                    Name = course.Name
                }).ToListAsync());
            }
            else
            {
                return Ok(await courses.Select(course => new CourseForClient
                {
                    Id = course.Id,
                    Description = course.Description,
                    Name = course.Name,
                    Created = course.Created
                }).ToListAsync());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseForCreation courseForCreation)
        {
            Course course = new Course
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

        [HttpDelete]
        public async Task<IActionResult> DeleteCourse([FromHeader(Name = "X-Course-Id")] int courseId)
        {
            Course? course = await dbContext.Courses.FirstOrDefaultAsync(course => course.Id == courseId && course.SchoolId == userContext.SchoolId);
            if (course != null)
            {
                dbContext.Courses.Remove(course);
                dbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
