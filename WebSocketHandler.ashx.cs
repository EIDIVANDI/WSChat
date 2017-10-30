using System;
using System.Web;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace WebSocketHandlerExample
{
    public class WebSocketHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(WebSocketProcess);
            }
        }

        public bool IsReusable { get { return false; } }

        private async Task WebSocketProcess(AspNetWebSocketContext context)
        {
            while (true)
            {
                ArraySegment<byte> arraySegment = new ArraySegment<byte>(new byte[1024]);
                // open the result. This is waiting asynchronously
                WebSocketReceiveResult socketResult =
                await context.WebSocket.ReceiveAsync(arraySegment,
                CancellationToken.None);
                // return the message to the client if the socket is still open
                if (context.WebSocket.State == WebSocketState.Open)
                {
                    string message = Encoding.UTF8.GetString(arraySegment.Array, 0,
                    socketResult.Count);
                    message = "Your message: " + message + " at " +
                    DateTime.Now.ToString();
                    arraySegment = new
                    ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                    // Asynchronously send a message to the client
                    await context.WebSocket.SendAsync(arraySegment,
                    WebSocketMessageType.Text,
                    true, CancellationToken.None);
                }
                else { break; }
            }
        }
    }
}