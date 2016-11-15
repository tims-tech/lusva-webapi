using System;
using Microsoft.AspNetCore.Builder;

namespace LUSVA.WebApi.Middleware
{
  public static class LusvaJwtBearerAppBuilderExtensions
  {
    public static IApplicationBuilder UseLusvaJwtBearerAuthentication(this IApplicationBuilder app, LusvaJwtBearerOptions options)
    {
      if (app == null)
        throw new ArgumentNullException(nameof(app));
      if (options == null)
        throw new ArgumentNullException(nameof(options));
      return
        app.UseMiddleware<LusvaJwtBearerMiddleware>(
          (object) Microsoft.Extensions.Options.Options.Create<LusvaJwtBearerOptions>(options));
    }
  }
}