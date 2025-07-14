using Tendance.API.Entities;
using Tendance.API.Models;

namespace Tendance.API.Abstractions
{
    public interface ICaptureDeviceHandler
    {
        Task<CaptureMatchResult> MatchAsync(byte[] data, CaptureDeviceEntity device);
        Task<CaptureRegisterResult> RegisterAsync(byte[] data, CaptureDeviceEntity device, AttendanceRole role, object person);
    }
}
