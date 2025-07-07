using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace Tendance.API.Models
{
    public static class FaceEncoder
    {
        private static readonly InferenceSession _session = new InferenceSession("MLModels/glint360k_r50.onnx");

        public static float[] ExtractEmbedding(Mat faceMat)
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
