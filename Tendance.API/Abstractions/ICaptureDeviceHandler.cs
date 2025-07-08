using Tendance.API.Entities;
using Tendance.API.Models;

namespace Tendance.API.Abstractions
{
    public interface ICaptureDeviceHandler
    {
        Task<CaptureMatchResult> MatchAsync(byte[] data, CaptureDevice device);
        Task<CaptureRegisterResult> RegisterAsync(byte[] data, CaptureDevice device, AttendanceRole role, object person);
    }
}
