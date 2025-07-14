using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using Tendance.API.Abstractions;
using Tendance.API.Data;
using Tendance.API.Entities;
using Tendance.API.Models;

namespace Tendance.API.Services
{
    public class FacialRecognitionService(ApplicationDbContext dbContext) : ICaptureDeviceHandler
    {
        private const string ProtoPath = "MLModels/deploy.prototxt";
        private const string ModelPath = "MLModels/res10_300x300_ssd_iter_140000.caffemodel";
        private static readonly InferenceSession _session = new("MLModels/glint360k_r50.onnx");
        private static readonly Net? net = CvDnn.ReadNetFromCaffe(ProtoPath, ModelPath);

        public async Task<CaptureMatchResult> MatchAsync(byte[] data, CaptureDeviceEntity captureDevice)
        {
            Mat? face = DetectAndCropFace(data);
            if (face == null)
            {
                return new CaptureMatchResult
                {
                    Success = false,
                    Error = CaptureError.UnrecognizedOrMultiple,
                    Message = "Face Unrecognized or multiple faces detected"
                };
            }

            var queryEmbedding = ExtractEmbedding(face);

            List<FaceEntity> registeredFaces = await dbContext.Faces
                .Include(face => face.CaptureDevice)
                .Where(sf => sf.CaptureDevice != null && sf.CaptureDevice.SchoolId == captureDevice.SchoolId)
                .ToListAsync();

            if (registeredFaces.Count == 0)
            {
                return new CaptureMatchResult
                {
                    Success = false,
                    Error = CaptureError.NotRegistered,
                    Message = "No registered faces found"
                };
            }

            float threshold = 0.1f;
            FaceEntity? bestMatch = null;
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
                return new CaptureMatchResult
                {
                    Success = false,
                    Error = CaptureError.NoMatch,
                    Message = "No matching face found",
                };
            }

            if (bestMatch is StudentFaceEntity student)
            {
                return new CaptureMatchResult
                {
                    Success = true,
                    Role = AttendanceRole.Student,
                    MatchId = student.StudentId,
                };
            }
            else if (bestMatch is TeacherFaceEntity teacher)
            {
                return new CaptureMatchResult
                {
                    Success = true,
                    Role = AttendanceRole.Teacher,
                    MatchId = teacher.Id,
                };
            }

            return new CaptureMatchResult
            {
                Success = false,
                Error = CaptureError.UnknownFaceType,
                Message = "Unrecognized Face Type"
            };
        }

        public async Task<CaptureRegisterResult> RegisterAsync(byte[] data, CaptureDeviceEntity captureDevice, AttendanceRole role, object person)
        {
            Mat? face = DetectAndCropFace(data);
            if (face == null)
            {
                return new CaptureRegisterResult
                {
                    Success = false,
                    Error = CaptureError.UnrecognizedOrMultiple,
                    Message = "Face Unrecognized or multiple faces detected"
                };
            }

            float[] encoding = ExtractEmbedding(face);

            switch (role)
            {
                case AttendanceRole.Student:
                    await SaveStudentFace((StudentEntity)person, captureDevice, encoding);
                    break;
                case AttendanceRole.Teacher:
                    await SaveTeacherFace((TeacherEntity)person, captureDevice, encoding);
                    break;
                default:
                    return new CaptureRegisterResult
                    {
                        Success = false,
                        Error = CaptureError.UnknownFaceType,
                    };
            }

            return new CaptureRegisterResult
            {
                Success = true,
            };
        }

        private async Task SaveStudentFace(StudentEntity student, CaptureDeviceEntity device, float[] encoding)
        {
            var studentFace = new StudentFaceEntity
            {
                StudentId = student.Id,
                CaptureDeviceId = device.Id,
                Created = DateTime.UtcNow,
                Embedding = encoding,
            };
            await dbContext.Faces.AddAsync(studentFace);
            await dbContext.SaveChangesAsync();
        }

        private async Task SaveTeacherFace(TeacherEntity teacher, CaptureDeviceEntity device, float[] encoding)
        {
            var teacherFace = new TeacherFaceEntity
            {
                TeacherId = teacher.Id,
                CaptureDeviceId = device.Id,
                Created = DateTime.UtcNow,
                Embedding = encoding,
            };
            await dbContext.Faces.AddAsync(teacherFace);
            await dbContext.SaveChangesAsync();
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


        private static Mat? DetectAndCropFace(byte[] imageBytes)
        {
            var image = Cv2.ImDecode(imageBytes, ImreadModes.Color);
            if (image.Empty()) return null;

            var blob = CvDnn.BlobFromImage(image, 1.0, new Size(300, 300), new Scalar(104, 177, 123));
            ArgumentNullException.ThrowIfNull(net);
            net.SetInput(blob);
            var detections = net.Forward();

            var cols = image.Cols;
            var rows = image.Rows;
            var data = detections.Reshape(1, detections.Size(2));

            for (int i = 0; i < data.Rows; i++)
            {
                float confidence = data.At<float>(i, 2);
                if (confidence > 0.6)
                {
                    int x1 = (int)(data.At<float>(i, 3) * cols);
                    int y1 = (int)(data.At<float>(i, 4) * rows);
                    int x2 = (int)(data.At<float>(i, 5) * cols);
                    int y2 = (int)(data.At<float>(i, 6) * rows);
                    var rect = new Rect(x1, y1, x2 - x1, y2 - y1);
                    return new Mat(image, rect).Clone();
                }
            }

            return null;
        }


        private static float[] ExtractEmbedding(Mat faceMat)
        {
            Cv2.Resize(faceMat, faceMat, new Size(112, 112));
            Cv2.CvtColor(faceMat, faceMat, ColorConversionCodes.BGR2RGB);

            float[] inputData = new float[3 * 112 * 112];

            for (int y = 0; y < 112; y++)
            {
                for (int x = 0; x < 112; x++)
                {
                    var pixel = faceMat.At<Vec3b>(y, x);
                    int index = y * 112 + x;
                    inputData[index] = pixel.Item0 / 255.0f;
                    inputData[index + 112 * 112] = pixel.Item1 / 255.0f;
                    inputData[index + 2 * 112 * 112] = pixel.Item2 / 255.0f;
                }
            }

            var tensor = new DenseTensor<float>(inputData, new[] { 1, 3, 112, 112 });

            using var result = _session.Run([NamedOnnxValue.CreateFromTensor("input.1", tensor)]);

            var output = result.First().AsEnumerable<float>().ToArray();

            float norm = (float)Math.Sqrt(output.Select(x => x * x).Sum());
            return output.Select(x => x / norm).ToArray();
        }
    }
}
