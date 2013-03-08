using Microsoft.AspNet.SignalR;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace One.Application.ServiceStack
{
    public class OutgoingHubAttribute : Attribute, IHasResponseFilter
    {
        public string Name { get; set; }
        public string Method { get; set; }

        public void ResponseFilter(IHttpRequest req, IHttpResponse res, object responseDto)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Method)) return;
            var hub = GlobalHost.ConnectionManager.GetHubContext(Name);

            if (hub != null)
            {
                hub.Clients.All.Invoke(Method, new { Time = DateTime.Now.ToString("G"), Data = responseDto });
            }
        }

        public IHasResponseFilter Copy()
        {
            return this;
        }

        public int Priority
        {
            get
            {
                return -1;
            }
        }
    }
}