#region

using Microsoft.AspNetCore.Hosting;
using Web.Areas.Identity;

#endregion

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}