using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LUSVA.WebApi.Middleware
{
  public class LusvaTokenAuthenticationOptions : AuthenticationOptions, IOptions<LusvaTokenAuthenticationOptions>
  {
    /// <summary>
    /// Forces the token issuer be validated
    /// </summary>
    public bool ValidateIssuer { get; set; }

    /// <summary>
    /// Audience token can be used
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Forces the token audience be validated
    /// </summary>
    public bool ValidateAudience { get; set; }

    /// <summary>
    /// Timespan token is good for
    /// </summary>
    public TimeSpan Expiration { get; set; }

    /// <summary>
    /// Used to sign token
    /// </summary>
    public SigningCredentials SigningCredentials { get; set; }

    public LusvaTokenAuthenticationOptions Value => this;

    public static LusvaTokenAuthenticationOptions Debug(string key)
    {
      return new LusvaTokenAuthenticationOptions
      {
        ClaimsIssuer = "LUSVA Test Server",
        ValidateIssuer = false,
        Audience = "http://localhost:5000/",
        ValidateAudience = false,
        AuthenticationScheme = "Bearer",
        //AutomaticAuthenticate = true,
        //AutomaticChallenge = true,
        Expiration = TimeSpan.FromDays(14),
        SigningCredentials = new SigningCredentials(
          new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
          SecurityAlgorithms.HmacSha256
        )
      };
    }
  }
}