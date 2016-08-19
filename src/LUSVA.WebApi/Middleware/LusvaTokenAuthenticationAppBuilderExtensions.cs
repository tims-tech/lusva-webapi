using System;
using Microsoft.AspNetCore.Builder;

namespace LUSVA.WebApi.Middleware
{
  public static class LusvaTokenAuthenticationAppBuilderExtensions
  {
    public static IApplicationBuilder UseLusvaTokenAuth(this IApplicationBuilder app)
    {
      if (app == null)
        throw new ArgumentNullException(nameof(app));
      var options = (LusvaTokenAuthenticationOptions) app
        .ApplicationServices.GetService(typeof(LusvaTokenAuthenticationOptions));
      return app.UseLusvaTokenAuth(options);
    }

    public static IApplicationBuilder UseLusvaTokenAuth(this IApplicationBuilder app,
      LusvaTokenAuthenticationOptions options)
    {
      if (app == null)
        throw new ArgumentNullException(nameof(app));
      if (options == null)
        throw new ArgumentNullException(nameof(options));
      return app.UseMiddleware<LusvaTokenAuthenticationMiddleware>((object)
        Microsoft.Extensions.Options.Options.Create<LusvaTokenAuthenticationOptions>(options));
    }
  }
}