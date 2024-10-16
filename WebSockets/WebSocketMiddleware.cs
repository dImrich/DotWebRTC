using System.Net.WebSockets;

namespace DotWebRTC.WebSockets;

public class WebSocketMiddleware(
    RequestDelegate next,
    WebSocketHandler webSocketHandler,
    ILogger<WebSocketMiddleware> logger)
{
    private const int BufferSize = 1024 * 1024 * 1;



    public async Task Invoke(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
            return;

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await webSocketHandler.OnConnected(socket);

        try
        {
            await Receive(socket, async (result, buffer) =>
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                    case WebSocketMessageType.Binary:
                        await webSocketHandler.ReceiveAsync(socket, result, buffer);
                        break;
                    case WebSocketMessageType.Close:
                        await webSocketHandler.OnDisconnected(socket);
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
        catch (WebSocketException wsEx)
        {
            
            logger.LogError($"WebSockets error: {wsEx.Message} (code: {wsEx.WebSocketErrorCode})");
        }
    }



    private async Task Receive(System.Net.WebSockets.WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[BufferSize];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            handleMessage(result, buffer);
        }
    }
}