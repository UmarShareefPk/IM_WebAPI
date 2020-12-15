using System;
using System.Threading.Tasks;
using IM.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(IM.App_Start.Startup))]

namespace IM.App_Start
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new oAuthAppProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),
                AllowInsecureHttp = true
            };
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}

/*
Steps to add Owin token implementation

1- Add packages
    i -   Install-Package Microsoft.AspNet.WebApi.Owin
    ii -  Install-Package Microsoft.Owin.Host.SystemWeb
    iii - Install-Package Microsoft.AspNet.Identity.Owin
2 - Add Provider Class like oAuthAppProvider in provider folder
3 - Add Owin startup class like in this file.



*/