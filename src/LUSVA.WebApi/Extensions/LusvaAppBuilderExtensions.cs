using System;
using System.Text;
using LUSVA.WebApi.Entities;
using LUSVA.WebApi.Middleware;
using LUSVA.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace LUSVA.WebApi.Extensions
{
  public static class LusvaAppBuilderExtensions
  {
    public static IApplicationBuilder UseLusvaJwtBearerAuthenticationDebug(this IApplicationBuilder app, string key)
    {
      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

      var parameters = new TokenValidationParameters
      {
        IssuerSigningKey = signingKey,
        ValidateIssuerSigningKey = false,

        ValidIssuer = "LUSVA Test Server",
        ValidateIssuer = false,

        ValidAudience = "http://localhost:5000/",
        ValidateAudience = false,

        ClockSkew = TimeSpan.Zero,
        AuthenticationType = JwtBearerDefaults.AuthenticationScheme
      };

      var options = new LusvaJwtBearerOptions
      {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true,
        TokenValidationParameters = parameters,
        AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme,
        UserManager = (UserManager<User>) app.ApplicationServices.GetService(typeof(UserManager<User>)),
        TokenService = (ITokenService) app.ApplicationServices.GetService(typeof(ITokenService)),
        ValidateSecurityStamp = false
      };

      return app.UseLusvaJwtBearerAuthentication(options);
    }
  }
}