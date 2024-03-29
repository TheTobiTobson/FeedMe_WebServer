﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
// Attribute Routing
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
// CORS - ROP
#if DEBUG  
using System.Web.Http.Cors;
#endif

namespace WebServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //////CORS  - REMOVE ON PRODUCTION //////           
            //config.EnableCors();

#if DEBUG
            var cors = new EnableCorsAttribute("http://localhost:50627", "*", "*");
            config.EnableCors(cors);
#endif  
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            /* Web API routes */
            // Attrobute Routing
            config.MapHttpAttributeRoutes();

            //Convention-based Routing
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
