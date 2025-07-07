using Tendance.API.Models;

namespace Tendance.API.Abstractions
{
    public interface IDeviceCaptureHandler
    {
        CaptureResult HandleDeviceCapture(ReadOnlySpan<byte> data, int id);
    }
}
