using DotWebRTC.WebRtc;
using DotWebRTC.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebSocketManager();

var app = builder.Build();

app.UseDefaultFiles(); 
app.UseStaticFiles();

app.UseWebSockets();
app.MapWebSocketHandler("/webrtc", app.Services.GetService<WebRtcWebSocketHandler>());

app.Run();
