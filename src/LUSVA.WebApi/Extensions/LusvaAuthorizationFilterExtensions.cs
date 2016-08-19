
using LUSVA.WebApi.Filters;
using LUSVA.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LUSVA.WebApi.Extensions
{
  public static class LusvaAuthorizationFilterExtensions
  {
    public static LusvaAuthorizationFilter UseTokenService(this LusvaAuthorizationFilter filter, IServiceCollection services)
    {
      var serviceProvider = services.BuildServiceProvider();
      filter = new LusvaAuthorizationFilter(serviceProvider.GetService<ITokenService>(), filter.Policy);
      return filter;
    }
  }
}