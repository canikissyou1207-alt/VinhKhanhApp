public class DeviceInfo
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
}