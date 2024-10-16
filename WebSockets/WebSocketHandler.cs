using System.Net.WebSockets;
using System.Text;

namespace DotWebRTC.WebSockets;

public abstract class WebSocketHandler(WebSocketManager webSocketManager)
{
    protected WebSocketManager WebSocketManager { get; set; } = webSocketManager;



    public virtual async Task OnConnected(System.Net.WebSockets.WebSocket socket)
    {
        WebSocketManager.AddClient(socket);
    }



    public virtual async Task OnDisconnected(System.Net.WebSockets.WebSocket socket)
    {
        await WebSocketManager.RemoveClient(WebSocketManager.GetClientId(socket));
    }



    public async Task SendMessageAsync(System.Net.WebSockets.WebSocket socket, string message)
    {
        if (socket.State != WebSocketState.Open)
            return;

        await socket.SendAsync(
            buffer: new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None
        );
    }



    public async Task SendMessageAsync(string socketId, string message)
    {
        await SendMessageAsync(WebSocketManager.GetClient(socketId), message);
    }



    public async Task SendMessageToAllAsync(string message)
    {
        foreach (var pair in WebSocketManager.GetClients())
        {
            if (pair.Value.State == WebSocketState.Open)
                await SendMessageAsync(pair.Value, message);
        }
    }



    public abstract Task ReceiveAsync(System.Net.WebSockets.WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
}