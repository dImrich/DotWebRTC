using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace DotWebRTC.WebSockets;

public class WebSocketManager
{
    private readonly ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> _clients = [];



    private static string CreateClientId()
        => Guid.NewGuid().ToString();



    public System.Net.WebSockets.WebSocket GetClient(string id)
        => _clients.FirstOrDefault(p => p.Key == id).Value;



    public ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> GetClients()
        => _clients;



    public string GetClientId(System.Net.WebSockets.WebSocket socket)
        => _clients.FirstOrDefault(p => p.Value == socket).Key;



    public void AddClient(System.Net.WebSockets.WebSocket socket)
        => _clients.TryAdd(CreateClientId(), socket);



    public Task RemoveClient(string id)
    {
        if (!_clients.TryRemove(id, out var socket))
            throw new ArgumentException($"No such client (Id: ${id})");

        return socket.CloseAsync(
            closeStatus: WebSocketCloseStatus.NormalClosure,
            statusDescription: $"Closed by the {nameof(WebSocketManager)}",
            cancellationToken: CancellationToken.None
        );
    }
}