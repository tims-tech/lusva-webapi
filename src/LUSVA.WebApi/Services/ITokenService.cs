using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using LUSVA.WebApi.Entities;
using Newtonsoft.Json.Linq;

namespace LUSVA.WebApi.Services
{
  public interface ITokenService
  {
    Task<JObject> CreateNewTokenAsJsonAsync(User user);

    Task<ClaimsPrincipal> GetUserFromTokenAsync(string token);

    Task<ClaimsPrincipal> GetUserFromTokenAsync(JwtSecurityToken jwt);

    Task<Claim> GetSecurityStampFromTokenAsync(string token);

    Task<JwtSecurityToken> Unprotect(string token);

    bool IsBearerToken(string token);
  }
}