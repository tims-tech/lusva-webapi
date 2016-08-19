using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LUSVA.WebApi.Configurations;
using LUSVA.WebApi.Entities;
using Microsoft.AspNetCore.Http.Authentication.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LUSVA.WebApi.Services
{
  public class TokenService : ITokenService
  {
    private readonly TokenServiceConfiguration _config;

    public TokenService(TokenServiceConfiguration config)
    {
      _config = config;
    }

    public Task<JObject> CreateNewTokenAsJsonAsync(User user)
    {
      var issued = DateTime.UtcNow;
      var expires = issued.Add(_config.Expiration);

      // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
      // You can add other claims here, if you want:
      var claims = new Claim[]
      {
        new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
        new Claim(JwtRegisteredClaimNames.NameId, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, user.SecurityStamp),
        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(issued).ToString(), ClaimValueTypes.Integer64)
      };

      // Create the JWT and write it to a string
      var jwt = new JwtSecurityToken(
        issuer: _config.Issuer,
        audience: _config.Audience,
        claims: claims,
        notBefore: issued,
        expires: expires,
        signingCredentials: _config.SigningCredentials);
      var token = new JwtSecurityTokenHandler().WriteToken(jwt);

      var tokenObject = new JObject(
        new JProperty("access_token", token),
        new JProperty("token_type", "Bearer"),
        new JProperty(".issued", issued),
        new JProperty(".expires", expires));

      return Task.FromResult(tokenObject);
    }

    public Task<ClaimsPrincipal> GetUserFromTokenAsync(string token)
    {
      return GetUserFromTokenAsync(Unprotect(token).Result);
    }

    public Task<ClaimsPrincipal> GetUserFromTokenAsync(JwtSecurityToken jwt)
    {
      var identity = new ClaimsIdentity(jwt.Claims);
      var principal = new ClaimsPrincipal(identity);
      return Task.FromResult(principal);
    }

    public Task<JwtSecurityToken> Unprotect(string token)
    {
      try
      {
        return Task.FromResult(new JwtSecurityTokenHandler().ReadJwtToken(token));
      }
      catch (Exception)
      {
        throw new InvalidTokenException("Invalid Token");
      }
    }

    public bool IsBearerToken(string token)
    {
      return token.ToLower().Contains("bearer");
    }

    public long ToUnixEpochDate(DateTime date)
      => (long) Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
  }
}