using System;
using LUSVA.WebApi.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace LUSVA.WebApi.Attributes
{
  public class LusvaAuthorizeAttribute : AuthorizeAttribute
  {

    public LusvaAuthorizeAttribute() : base()
    {
      ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
    }
  }
}