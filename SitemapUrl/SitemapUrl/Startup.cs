using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;


[assembly: OwinStartup(typeof(SitemapUrl.Startup))]

namespace SitemapUrl
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
