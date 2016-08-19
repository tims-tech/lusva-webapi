

using System;
using System.Collections.Generic;
using System.Text;
using LUSVA.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LUSVA.WebApi.Providers
{
  public class TokenParametersProvider
  {
    public static TokenValidationParameters CreateTokenValidationParameters(string secret, string audience, string issuer)
    {
      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
      var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

      return new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,

        ValidateIssuer = false,
        ValidIssuer = issuer,

        ValidateAudience = true,
        ValidAudience = audience,

        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero,
        AuthenticationType = JwtBearerDefaults.AuthenticationScheme
      };
    }
  }
}