using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColoCanvas
{
    public class WebSocketsServer : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(new MicrosoftWebSockets());
            }
        }
    }
}