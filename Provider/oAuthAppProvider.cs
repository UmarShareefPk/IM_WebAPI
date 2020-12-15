using IM.Models;
using IM.SQL;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IM.Provider
{
    public class oAuthAppProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var username = context.UserName;
                var password = context.Password;

                UserLogin user = UsersMethods.Login(username, password);
                if (user != null)
                {
                    var claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.Name, "User"));
                    claims.Add(new Claim("Id", user.user.Id));



                    // var props = new AuthenticationProperties(new Dictionary<string, string>
                    // {
                    //     {
                    //         "client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    //     },
                    //     {
                    //         "Name", user.Name
                    //     }
                    // });


                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, IM.App_Start.Startup.OAuthOptions.AuthenticationType);
                    context.Validated(new AuthenticationTicket(oAutIdentity, new AuthenticationProperties() { }));
                }
                else
                {
                    context.SetError("invalid_grant", "Error");
                }
            });
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}