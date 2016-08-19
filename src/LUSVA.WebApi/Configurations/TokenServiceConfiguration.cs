using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LUSVA.WebApi.Configurations
{
  public class TokenServiceConfiguration
  {
    public string Issuer { get; set; }

    public string Audience { get; set; }

    public TimeSpan Expiration { get; set; }

    public SigningCredentials SigningCredentials { get; set; }

    public static string Key { get; set; }

    public static TokenServiceConfiguration DebugConfiguration
      => new TokenServiceConfiguration
    {
      Issuer = "LUSVA Test Server",
      Audience = "http://localhost:5000/",
      Expiration = TimeSpan.FromDays(14),
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
        SecurityAlgorithms.HmacSha256
      )
    };
  }
}