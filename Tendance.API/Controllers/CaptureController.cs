using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using Tendance.API.Data;
using Tendance.API.Entities;
using Tendance.API.Models;
using Tendance.API.Services;

namespace Tendance.API.Controllers
{
    [Route("api/capture")]
    [ApiController]
    public class CaptureController(ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CaptureAsync([FromHeader(Name = "X-Client-Key")] string clientKey, [FromForm] IFormFile formFile)
        {
            CaptureDevice? device = await dbContext.Devices.FirstOrDefaultAsync(device => device.ClientKey == clientKey);
            if (device == null)
            {
                return BadRequest("Unknown Device");
            }

            if (device.ClassroomId == null)
            {
                return BadRequest("Classroom not set");
            }

            using var ms = new MemoryStream();
            await formFile.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            Mat? face = FaceDetectionHelper.DetectAndCropFace(bytes);
            if (face == null)
            {
                return BadRequest("Face Unrecognized or multiple faces detected");
            }

            var queryEmbedding = FaceEncoder.ExtractEmbedding(face);

            var registeredFaces = await dbContext.StudentFaces
                .Include(sf => sf.Student)
                .Where(sf => sf.Student.SchoolId == device.SchoolId)
                .ToListAsync();

            if (registeredFaces.Count == 0)
            {
                return NotFound("No registered faces found");
            }

            float threshold = 0.1f;
            StudentFace? bestMatch = null;
            float bestScore = -1f;

            foreach (var regFace in registeredFaces)
            {
                float score = CosineSimilarity(queryEmbedding, regFace.Embedding);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = regFace;
                }
            }

            if (bestMatch == null || bestScore < threshold)
            {
                return Ok(new { Matched = false, Message = "No matching face found" });
            }

            // Matched!
            return Ok(new
            {
                Matched = true,
                StudentId = bestMatch.StudentId,
                Similarity = bestScore
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(
            [FromHeader(Name = "X-Client-Key")] string clientKey,
            [FromHeader(Name = "X-Student-Id")] string studentSchoolId,
            [FromForm] IFormFile formFile)
        {
            CaptureDevice? device = await dbContext.Devices.FirstOrDefaultAsync(device => device.ClientKey == clientKey);
            if (device == null)
            {
                return BadRequest("Unknown Device");
            }

            Student? student = await dbContext.Students.FirstOrDefaultAsync(student => student.SchoolAssignedId == studentSchoolId && student.SchoolId == device.SchoolId);
            if (student == null)
            {
                return BadRequest("Unknown Student");
            }

            using var ms = new MemoryStream();
            await formFile.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            Mat? face = FaceDetectionHelper.DetectAndCropFace(bytes);
            if (face == null)
            {
                return BadRequest("Face Unrecognized");
            }

            var encoding = FaceEncoder.ExtractEmbedding(face);
            var studentFace = new StudentFace
            {
                StudentId = student.Id,
                CaptureDeviceId = device.Id,
                Created = DateTime.UtcNow,
                Embedding = encoding,
            };

            await dbContext.StudentFaces.AddAsync(studentFace);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        private static float CosineSimilarity(float[] a, float[] b)
        {
            float dot = 0f, normA = 0f, normB = 0f;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }
            return dot / ((float)Math.Sqrt(normA) * (float)Math.Sqrt(normB));
        }
    }
}
