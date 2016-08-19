using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LUSVA.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LUSVA.WebApi.Filters
{
  public class LusvaAuthorizationFilter : AuthorizeFilter
  {
    private readonly ITokenService _tokenService;

    public LusvaAuthorizationFilter(ITokenService tokenService, AuthorizationPolicy policy) : base(policy)
    {
      Console.WriteLine("Setting token service...");
      _tokenService = tokenService;
    }

    public LusvaAuthorizationFilter(ITokenService tokenService, IAuthorizationPolicyProvider policyProvider,
      IEnumerable<IAuthorizeData> authorizeData) : base(policyProvider, authorizeData)
    {
      _tokenService = tokenService;
    }

    public LusvaAuthorizationFilter(AuthorizationPolicy policy) : base(policy)
    {
      Console.WriteLine("Setting up filter...");
      _tokenService = null;
    }

    public LusvaAuthorizationFilter(IAuthorizationPolicyProvider policyProvider,
      IEnumerable<IAuthorizeData> authorizeData) : base(policyProvider, authorizeData)
    {
      _tokenService = null;
    }

    public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
//      Console.WriteLine("Authorizing request...");
//      // If the token service is set, authorizing will be token based
//      if (_tokenService != null)
//      {
//        Console.WriteLine("Using token authentication...");
//        var headers = context.HttpContext.Request.Headers;
//        if (!headers.ContainsKey("Authorization"))
//          throw new ArgumentException("Missing Token");
//        var token = headers["Authorization"][0];
//        if (!_tokenService.IsBearerToken(token))
//          throw new InvalidTokenException("Not a Bearer token");
//        token = token.Split(' ').Last(); // strip Bearer from string, only need token
//        context.HttpContext.User = await _tokenService.GetUserFromTokenAsync(token);
//        Console.WriteLine("Request authenticated...");
//      }
//      else // otherwise, cookie authentication is used
//        await base.OnAuthorizationAsync(context);
    }
  }
}