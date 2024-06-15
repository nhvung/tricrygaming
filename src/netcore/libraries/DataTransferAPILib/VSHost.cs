using System;
using Microsoft.AspNetCore.Hosting;
using VSSystem.Extensions.Hosting;
using VSSystem.Logger;
using VSSystem.ServiceProcess.Extensions;

namespace DataTransferAPILib
{
    public class VSHost : AWebHost
    {
        public static string SERVICE_NAME = null;
        public static string PRIVATE_KEY = null;
        public VSHost(string name, string displayName, int port, string privateKey)
            : base(name, displayName, port, privateKey)
        {
        }
        public static ALogger StaticLogger = null;
        protected override void _InitializeLogger()
        {
            base._InitializeLogger();
            StaticLogger = _logger;
        }

        protected override void _UseStartup(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseStartup<VSStartup>();
        }
        protected override void _UseConfiguration(string[] args)
        {
            base._UseConfiguration(args);
            SERVICE_NAME = _Name;
            PRIVATE_KEY = _privateKey;
        }
    }
}