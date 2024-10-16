using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using DotWebRTC.WebSockets;
using SDPLib;
using WebSocketManager = DotWebRTC.WebSockets.WebSocketManager;

namespace DotWebRTC.WebRtc;

public class WebRtcWebSocketHandler(WebSocketManager ws, ILogger<WebRtcWebSocketHandler> logger) : WebSocketHandler(ws)
{
    public override async Task OnConnected(WebSocket socket)
    {
        await base.OnConnected(socket);

        var socketId = WebSocketManager.GetClientId(socket);
        logger.LogInformation($"{socketId} is now connected");
    }



    public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        var socketId = WebSocketManager.GetClientId(socket);
        
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        
        await SendMessageToAllAsync(message);
        
        LogWebRtc(socketId, message);
    }



    private void LogWebRtc(string socketId, string message)
    {
        var webRtcMessage = JsonSerializer.Deserialize<WebRtcMessage>(message);
        if (webRtcMessage == null)
        {
            logger.LogError("Unable to parse SDP message.");
            return;
        }

        if (webRtcMessage.Sdp != null)
        {
            var logPrefix = $"[SDP_{webRtcMessage.Sdp.Type}][{socketId}]";
            logger.LogInformation(logPrefix);

            var sdp = SDPSerializer.ReadSDP(Encoding.ASCII.GetBytes(webRtcMessage.Sdp.Sdp));
            if (sdp == null)
            {
                logger.LogError("Unable to parse SDP packet.");
                return;
            }
            
            foreach (var mediaDesc in sdp.MediaDescriptions)
            {
                logger.LogInformation($"{logPrefix} Media={mediaDesc.Media}, Proto={mediaDesc.Proto}, Port={mediaDesc.Port}");
            }
            
            var sdpJson = JsonSerializer.SerializeToUtf8Bytes(sdp);
            
            //logger.LogInformation($"{socketId} sent SDP {webRtcMessage.Sdp.Type}: \n {Encoding.UTF8.GetString(sdpJson)}");
        }

        if (webRtcMessage.Ice != null)
        {
            var logPrefix = $"[ICE][{socketId}]";

            logger.LogInformation($"{logPrefix} Candidate={webRtcMessage.Ice.Candidate}");
        }
    }
}