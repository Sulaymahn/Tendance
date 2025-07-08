namespace Tendance.API.Models
{
    public struct CaptureRegisterResult
    {
        public bool Success { get; set; }
        public CaptureError Error { get; set; }
        public string Message { get; set; }
    }
}
