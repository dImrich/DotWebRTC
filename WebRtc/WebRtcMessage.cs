using System.Text.Json.Serialization;

namespace DotWebRTC.WebRtc;


public record WebRtcMessage
{
    [JsonPropertyName("sdp")]
    public SdpPacket? Sdp { get; init; }
    
    [JsonPropertyName("ice")]
    public IcePacket? Ice { get; init; }
    
    [JsonPropertyName("uuid")]
    public string Uuid { get; init; }
}

public record SdpPacket
{
    [JsonPropertyName("type")]
    public string Type { get; init; }
    
    [JsonPropertyName("sdp")]
    public string Sdp { get; init; }
}
public record IcePacket
{
    [JsonPropertyName("candidate")]
    public string Candidate { get; init; }
    
    [JsonPropertyName("usernameFragment")]
    public string UsernameFragment { get; init; }
    
    [JsonPropertyName("sdpMid")]
    public string SdpMid { get; init; }
    
    [JsonPropertyName("sdpMLineIndex")]
    public int SdpMLineIndex { get; init; }
}