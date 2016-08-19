using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using LUSVA.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

namespace LUSVA.WebApi.Middleware
{
  public class LusvaTokenAuthenticationHandler : AuthenticationHandler<LusvaTokenAuthenticationOptions>
  {
    private readonly ITokenService _tokenService;

    public LusvaTokenAuthenticationHandler(ITokenService tokenService)
    {
      _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      var headers = this.Context.Request.Headers;
      if (!headers.ContainsKey("Authorization"))
        return await FailAsync("Missing Token");
      var tokenHeader = headers["Authorization"][0];
      if (!_tokenService.IsBearerToken(tokenHeader))
        return await FailAsync("Not Bearer Token");
      var token = await _tokenService.Unprotect(tokenHeader.Split(' ').Last());
      try { ValidateToken(token); }
      catch (InvalidTokenException e) { return await FailAsync(e.Message); }
      return await CreateResult(token);
    }

    private void ValidateToken(JwtSecurityToken jwt)
    {
      if (this.Options.ValidateIssuer)
        ThrowIfTrue(jwt.Issuer == this.Options.ClaimsIssuer, "Invalid Issuer");
      if (this.Options.ValidateAudience)
        ThrowIfTrue(jwt.Audiences.Any(s => s == this.Options.Audience), "Invalid Audience");
    }

    private async Task<AuthenticateResult> CreateResult(JwtSecurityToken jwt)
    {
      var principal = await _tokenService.GetUserFromTokenAsync(jwt);
      var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), "Token");
      return AuthenticateResult.Success(ticket);
    }

    private Task<AuthenticateResult> FailAsync(string failMessage)
    {
      return Task.FromResult(AuthenticateResult.Fail(failMessage));
    }

    private void ThrowIfTrue(bool doThrow, string throwMessage)
    {
      if (doThrow)
        throw new InvalidTokenException(throwMessage);
    }
  }
}