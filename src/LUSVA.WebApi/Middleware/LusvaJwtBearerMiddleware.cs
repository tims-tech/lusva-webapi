using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LUSVA.WebApi.Middleware
{
  public class LusvaJwtBearerMiddleware : JwtBearerMiddleware
  {
    public LusvaJwtBearerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
      UrlEncoder encoder, IOptions<LusvaJwtBearerOptions> options) : base(next, loggerFactory, encoder, options)
    {
    }

    protected override AuthenticationHandler<JwtBearerOptions> CreateHandler()
    {
      JwtBearerOptions options = new LusvaJwtBearerOptions();
      return new LusvaJwtBearerHandler();
    }
  }
}