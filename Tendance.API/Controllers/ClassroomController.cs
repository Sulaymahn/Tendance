using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tendance.API.Authentication;
using Tendance.API.Data;
using Tendance.API.DataTransferObjects.Classroom;
using Tendance.API.DataTransferObjects.Student;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{DeviceAuthDefaults.AuthenticationScheme}")]
    [Route("api/classrooms")]
    [ApiController]
    public class ClassroomController(ApplicationDbContext dbContext, UserContextAccessor userContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetClassrooms()
        {
            List<ClassroomForClient> classrooms = await dbContext.Classrooms
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Include(c => c.Room)
                .Where(c => c.SchoolId == userContext.SchoolId)
                .Select(c => new ClassroomForClient
                {
                    Id = c.Id,
                    Created = c.Created,
                    Course = new ClassroomCourse
                    {
                        Id = c.Course!.Id,
                        Name = c.Course.Name,
                    },
                    Room = new ClassroomRoom
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
                        Id = student.Id,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        MiddleName = student.MiddleName,
                    })
                })
            .ToListAsync();

            return Ok(classrooms);
        }

        [HttpGet("{classroomId:int}")]
        public async Task<IActionResult> GetClassroomById([FromRoute] int classroomId)
        {
            ClassroomForClient? classroom = await dbContext.Classrooms
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Include(c => c.Room)
                .Include(c => c.Students)
                .Where(c => c.SchoolId == userContext.SchoolId && c.Id == classroomId)
                .Select(c => new ClassroomForClient
                {
                    Id = c.Id,
                    Created = c.Created,
                    Course = new ClassroomCourse
                    {
                        Id = c.Course!.Id,
                        Name = c.Course.Name,
                    },
                    Room = new ClassroomRoom
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
                        Id = student.Id,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        MiddleName = student.MiddleName,
                    })
                })
            .FirstOrDefaultAsync();

            if (classroom == null)
            {
                return NotFound();
            }

            return Ok(classroom);
        }

        [HttpGet("{classroomId:int}/students")]
        public async Task<IActionResult> GetClassroomStudents([FromRoute] int classroomId)
        {
            List<ClassroomStudent> classrooms = await dbContext.Classrooms
                .Where(c => c.SchoolId == userContext.SchoolId && c.Id == classroomId)
                .SelectMany(c => c.Students.Select(student => new ClassroomStudent
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    MiddleName = student.MiddleName,
                }))
                .ToListAsync();

            return Ok(classrooms);
        }

        [HttpPost("{classroomId:int}/students")]
        public async Task<IActionResult> AddClassroomStudents([FromRoute] int classroomId, [FromBody] List<int> studentIds)
        {

            if (studentIds == null || studentIds.Any())
            {
                return BadRequest("No student IDs provided.");
            }

            var classroomExists = await dbContext.Classrooms.AnyAsync(c => c.Id == classroomId);
            if (!classroomExists)
            {
                return NotFound($"Classroom with ID {classroomId} not found.");
            }

            var existingStudentClassrooms = await dbContext.ClassroomStudents
                .Where(sc => sc.ClassroomId == classroomId && studentIds.Contains(sc.StudentId))
                .Select(sc => sc.StudentId)
                .ToListAsync();

            var studentIdsToAdd = studentIds
                .Except(existingStudentClassrooms)
                .ToList();

            if (!studentIdsToAdd.Any())
            {
                return Ok("All provided students are already assigned to this classroom.");
            }

            var validStudentIds = await dbContext.Students
                .Where(s => studentIdsToAdd.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var invalidStudentIds = studentIdsToAdd.Except(validStudentIds).ToList();
            if (invalidStudentIds.Any())
            {
                return BadRequest($"The following student IDs are invalid or do not exist: {string.Join(", ", invalidStudentIds)}");
            }

            var newStudentClassrooms = validStudentIds.Select(studentId => new ClassroomStudentEntity
            {
                StudentId = studentId,
                ClassroomId = classroomId,
            }).ToList();

            await dbContext.ClassroomStudents.AddRangeAsync(newStudentClassrooms);

            await dbContext.SaveChangesAsync();

            return Ok($"Successfully added {newStudentClassrooms.Count} student(s) to classroom {classroomId}.");
        }

        [HttpPut("{classroomId:int}/students")]
        public async Task<IActionResult> UpdateClassroomStudents([FromRoute] int classroomId, [FromBody] List<int> studentIds)
        {
            var classroomExists = await dbContext.Classrooms.AnyAsync(c => c.Id == classroomId);
            if (!classroomExists)
            {
                return NotFound($"Classroom with ID {classroomId} not found.");
            }

            await dbContext.ClassroomStudents
                .Where(sc => sc.ClassroomId == classroomId)
                .ExecuteDeleteAsync();

            var validStudentIds = await dbContext.Students
                .Where(s => studentIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var invalidStudentIds = studentIds.Except(validStudentIds).ToList();
            if (invalidStudentIds.Any())
            {
                return BadRequest($"The following student IDs are invalid or do not exist: {string.Join(", ", invalidStudentIds)}");
            }

            var newStudentClassrooms = validStudentIds.Select(studentId => new ClassroomStudentEntity
            {
                StudentId = studentId,
                ClassroomId = classroomId,
            }).ToList();

            await dbContext.ClassroomStudents.AddRangeAsync(newStudentClassrooms);

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = TendancePolicy.UserOnly)]
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

            var classroom = new ClassroomEntity
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
                Room = new ClassroomRoom
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

        [Authorize(Policy = TendancePolicy.UserOnly)]
        [HttpDelete("{classroomId:int}")]
        public async Task<IActionResult> DeleteClassroom([FromRoute] int classroomId)
        {
            var classroom = await dbContext.Classrooms
                .FirstOrDefaultAsync(c => c.SchoolId == userContext.SchoolId && c.Id == classroomId);

            if (classroom == null)
            {
                return NotFound();
            }

            dbContext.Classrooms.Remove(classroom);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
