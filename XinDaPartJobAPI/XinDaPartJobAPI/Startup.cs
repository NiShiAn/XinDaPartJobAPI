﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(XinDaPartJobAPI.Startup))]

namespace XinDaPartJobAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
