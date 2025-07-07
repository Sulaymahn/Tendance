using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace Tendance.API.Models
{
    public static class FaceDetectionHelper
    {
        private static readonly string ProtoPath = "MLModels/deploy.prototxt";
        private static readonly string ModelPath = "MLModels/res10_300x300_ssd_iter_140000.caffemodel";

        public static Mat? DetectAndCropFace(byte[] imageBytes)
        {
            var image = Cv2.ImDecode(imageBytes, ImreadModes.Color);
            if (image.Empty()) return null;

            var blob = CvDnn.BlobFromImage(image, 1.0, new Size(300, 300), new Scalar(104, 177, 123));
            var net = CvDnn.ReadNetFromCaffe(ProtoPath, ModelPath);
            net.SetInput(blob);
            var detections = net.Forward();

            var cols = image.Cols;
            var rows = image.Rows;
            var data = detections.Reshape(1, detections.Size(2));

            for (int i = 0; i < data.Rows; i++)
            {
                float confidence = data.At<float>(i, 2);
                if (confidence > 0.6) // adjust threshold
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
    }
}
