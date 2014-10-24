using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class WebsocketClient : Client
    {
        internal Alchemy.Classes.UserContext context;

        public override void Send(string message)
        {
            if (context != null && context.DataFrame != null) context.Send(message);
        }

        public override string ConnectionDescription
        {
            get
            {
                if (context != null) return context.ClientAddress.ToString();
                return "NOT CONNECTED";
            }
        }

        public override string IPString
        {
            get
            {
                var desc = ConnectionDescription;
                var split = desc.IndexOf(':');
                if (split == -1) return "0.0.0.0";
                return desc.Substring(0, split);
            }
        }

        override public void Disconnect()
        {
            if (context != null)
            {
                Console.WriteLine("Websocket client left gracefully : " + context.ClientAddress.ToString());
                // ???
            }
        }
    }
}
