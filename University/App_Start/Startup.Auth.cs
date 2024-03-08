using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace University
{
    public partial class Startup
    {

        public static string OidcAuthority = ConfigurationManager.AppSettings["oidc:Authority"];
        public static string OidcRedirectUrl = ConfigurationManager.AppSettings["oidc:RedirectUrl"];
        public static string OidcClientId = ConfigurationManager.AppSettings["oidc:ClientId"];
        public static string OidcClientSecret = ConfigurationManager.AppSettings["oidc:ClientSecret"];

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            //app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/Account/Login"),
            //    CookieHttpOnly = true,
            //});
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                Authority = OidcAuthority,
                ClientId = OidcClientId,
                ClientSecret = OidcClientSecret,
                PostLogoutRedirectUri = OidcRedirectUrl,
                RedirectUri = OidcRedirectUrl,
                ResponseType = OpenIdConnectResponseType.Code,
                Scope = OpenIdConnectScope.OpenId,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "resource_access"
                },
                SaveTokens = true,
                RedeemCode = true,
                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = async n =>
                    {
                        switch (n.ProtocolMessage.RequestType)
                        {
                            case OpenIdConnectRequestType.Logout:
                                n.ProtocolMessage.SetParameter("client_id", OidcClientId);
                                break;
                        }

                        n.Response.Cookies.Append("redirectAction", HttpContext.Current.Request.Url.AbsolutePath);
                        //n.ProtocolMessage.SetParameter("redirect_uri", HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
                        //n.ProtocolMessage.RedirectUri = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
                    }
                }

            };
            app.UseOpenIdConnectAuthentication(oidcOptions);



            
            //app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            //{
            //    ClientId = OidcClientId,
            //    Authority = OidcAuthority,
            //    ClientSecret= OidcClientSecret,
            //    RedirectUri = OidcRedirectUrl,
            //    ResponseType = OpenIdConnectResponseType.Code,
            //    Scope = OpenIdConnectScope.OpenId,

            //    SignInAsAuthenticationType = "Cookies",
            //});

            // Enable the application to use a cookie to store information for the signed in user
            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/Account/Login")
            //});
            //// Use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }

        public async Task<string> GetDynamicRedirectUriAsync(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            // Perform your asynchronous logic here to determine the appropriate redirect URI.
            // For example, you can check the request URL and return a different URI accordingly.
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        public async Task<string> GetDynamicCallbackUriAsync(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            // Perform your asynchronous logic here to determine the appropriate redirect URI.
            // For example, you can check the request URL and return a different URI accordingly.
            return HttpContext.Current.Request.Url.AbsolutePath;
        }

    }

}