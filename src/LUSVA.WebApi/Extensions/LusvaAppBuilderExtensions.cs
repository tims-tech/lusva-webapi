using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;

namespace LUSVA.WebApi.Extensions
{
  public static class LusvaAppBuilderExtensions
  {
    public static IApplicationBuilder UseJwtBearerAuthenticationDebug(this IApplicationBuilder app, string key)
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

      var options = new JwtBearerOptions
      {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true,
        TokenValidationParameters = parameters,
        AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme,
      };

      return app.UseJwtBearerAuthentication(options);
    }
  }
}