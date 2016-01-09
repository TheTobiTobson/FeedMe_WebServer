/***************/
// Execution: This file is called first
// Main Purpose:
//
/***************/


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

// Set Startup Class - T
[assembly: OwinStartup(typeof(WebServer.Startup))]

namespace WebServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Call ConfigureAuth(app) right after start
            // ConfigureAuth(app) is defined in App_Start/IdentityModels.cs
            ConfigureAuth(app);
        }
    }
}
