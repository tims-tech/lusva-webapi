using LUSVA.WebApi.Entities;
using LUSVA.WebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace LUSVA.WebApi.Middleware
{
  public class LusvaJwtBearerOptions : JwtBearerOptions
  {
    public ITokenService TokenService { get; set; }
    public UserManager<User> UserManager { get; set; }
    public bool ValidateSecurityStamp { get; set; }
  }
}