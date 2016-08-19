using LUSVA.WebApi.Configurations;
using LUSVA.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LUSVA.WebApi.Middleware
{
  public static class LusvaTokenAuthenticationServiceCollectionExtensions
  {

    public static IServiceCollection AddLusvaTokenAuth(this IServiceCollection services, string key, bool useDebug)
    {
      return services
        .AddScoped<ITokenService, TokenService>(provider =>
            new TokenService(TokenServiceConfiguration.DebugConfiguration))
        .AddScoped<LusvaTokenAuthenticationOptions, LusvaTokenAuthenticationOptions>(
          x => LusvaTokenAuthenticationOptions.Debug(key) // todo: fix for useDebug
        );
    }

  }
}