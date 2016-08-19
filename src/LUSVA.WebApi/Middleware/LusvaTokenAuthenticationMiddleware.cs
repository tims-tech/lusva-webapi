using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LUSVA.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LUSVA.WebApi.Middleware
{
  public class LusvaTokenAuthenticationMiddleware : AuthenticationMiddleware<LusvaTokenAuthenticationOptions>
  {
    private readonly RequestDelegate _next;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;

    public LusvaTokenAuthenticationMiddleware(
      RequestDelegate next,
      IOptions<LusvaTokenAuthenticationOptions> options,
      ITokenService tokenService,
      ILoggerFactory loggerFactory,
      UrlEncoder urlEncoder) : base(next, options, loggerFactory, urlEncoder)
    {
      _tokenService = tokenService;
    }


    protected override AuthenticationHandler<LusvaTokenAuthenticationOptions> CreateHandler()
    {
      return (AuthenticationHandler<LusvaTokenAuthenticationOptions>) new LusvaTokenAuthenticationHandler(_tokenService);
    }
  }
}