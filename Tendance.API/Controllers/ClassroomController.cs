using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Classroom;
using Tendance.API.DataTransferObjects.Course;
using Tendance.API.DataTransferObjects.Room;
using Tendance.API.DataTransferObjects.Student;
using Tendance.API.DataTransferObjects.Teacher;
using Tendance.API.Entities;
using Tendance.API.Services;
using ClassroomStudent = Tendance.API.DataTransferObjects.Classroom.ClassroomStudent;

namespace Tendance.API.Controllers
{
    [Route("api/classrooms")]
    [Authorize]
    [ApiController]
    public class ClassroomController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetClassrooms([FromHeader(Name = "X-Minimal")] bool? minimal)
        {
            IQueryable<Classroom> classrooms = dbContext.Classrooms
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Include(c => c.Room)
                .Where(c => c.SchoolId == userContext.SchoolId);

            if (minimal.HasValue && minimal == true)
            {
                return Ok(await classrooms.Select(c => new ClassroomForClientMinimal
                {
                    Id = c.Id,
                    Course = new CourseForClientMinimal
                    {
                        Id = c.Course!.Id,
                        Name = c.Course.Name,
                    },
                    Room = new RoomForClientMinimal
                    {
                        Id = c.Room!.Id,
                        Name = c.Room.Name,
                        Building = c.Room.Building,
                    },
                    Teacher = new TeacherForClientMinimal
                    {
                        Id = c.Teacher!.Id,
                        FirstName = c.Teacher.FirstName,
                        LastName = c.Teacher.LastName,
                        MiddleName = c.Teacher.MiddleName,
                    },
                    Students = c.Students.Select(student => new StudentForClientMinimal
                    {
                        Id = student.SchoolAssignedId,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        MiddleName = student.MiddleName,
                    })
                })
                .ToListAsync());
            }
            else
            {
                return Ok(await classrooms.Select(c => new ClassroomForClient
                {
                    Id = c.Id,
                    Created = c.Created,
                    Course = new ClassroomCourse
                    {
                        Id = c.Course!.Id,
                        Name = c.Course.Name,
                    },
                    Room = new RoomForClient
                    {
                        Id = c.Room!.Id,
                        Name = c.Room.Name,
                        Building = c.Room.Building,
                    },
                    Teacher = new ClassroomTeacher
                    {
                        Id = c.Teacher!.Id,
                        FirstName = c.Teacher.FirstName,
                        LastName = c.Teacher.LastName,
                        MiddleName = c.Teacher.MiddleName,
                    },
                    Students = c.Students.Select(student => new ClassroomStudent
                    {
                        Id = student.SchoolAssignedId,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        MiddleName = student.MiddleName,
                    })
                })
                .ToListAsync());
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClassroomForClient>> CreateClassroom([FromBody] ClassroomForCreation classroomForCreation)
        {
            var course = await dbContext.Courses.FirstOrDefaultAsync(c => c.SchoolId == userContext.SchoolId && c.Id == classroomForCreation.CourseId);
            if (course == null)
            {
                return BadRequest("Course unavailable");
            }

            var teacher = await dbContext.Teachers.FirstOrDefaultAsync(c => c.SchoolId == userContext.SchoolId && c.Id == classroomForCreation.TeacherId);
            if (teacher == null)
            {
                return BadRequest("Teacher unavailable");
            }

            var room = await dbContext.Rooms.FirstOrDefaultAsync(c => c.SchoolId == userContext.SchoolId && c.Id == classroomForCreation.RoomId);
            if (room == null)
            {
                return BadRequest("Room unavailable");
            }

            var classroom = new Classroom
            {
                Created = DateTime.UtcNow,
                CourseId = course.Id,
                RoomId = room.Id,
                SchoolId = userContext.SchoolId,
                TeacherId = teacher.Id,
            };

            dbContext.Classrooms.Add(classroom);
            await dbContext.SaveChangesAsync();

            var result = new ClassroomForClient
            {
                Created = classroom.Created,
                Id = classroom.Id,
                Course = new ClassroomCourse
                {
                    Id = course.Id,
                    Name = course.Name,
                },
                Room = new RoomForClient
                {
                    Id = room.Id,
                    Building = room.Building,
                    Name = room.Name,
                },
                Teacher = new ClassroomTeacher
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    MiddleName = teacher.MiddleName,
                },
            };

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteClassroom([FromHeader(Name = "X-Classroom-Id")] int id)
        {
            var schoolId = userContext.SchoolId;
            var classroom = await dbContext.Classrooms
                .FirstOrDefaultAsync(c => c.SchoolId == schoolId && c.Id == id);

            if (classroom == null)
                return NotFound();

            dbContext.Classrooms.Remove(classroom);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
