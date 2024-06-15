using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using VSSystem.Extensions.Hosting;

namespace DataTransferAPILib
{
    public class VSStartup : AStartup
    {
        public VSStartup() : base()
        {
            _useWebRoot = true;
        }
        protected override void _ConfigureMiddleware(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<PrefixPathMiddleware>(VSHost.SERVICE_NAME, VSHost.PRIVATE_KEY);
        }
    }
}
