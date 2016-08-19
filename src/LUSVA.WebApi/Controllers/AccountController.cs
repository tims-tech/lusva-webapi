using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LUSVA.WebApi.Attributes;
using LUSVA.WebApi.Configurations;
using LUSVA.WebApi.Entities;
using LUSVA.WebApi.Models;
using LUSVA.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace LUSVA.WebApi.Controllers
{
  [Route("api/account")]
  public class AccountController : Controller
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;

    public AccountController(
      UserManager<User> userManager,
      SignInManager<User> signInManager,
      IEmailSender emailSender,
      ITokenService tokenService,
      ILoggerFactory loggerFactory)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _emailSender = emailSender;
      _tokenService = tokenService;
      _logger = loggerFactory.CreateLogger<AccountController>();
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(SignInModel model)
    {
      var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
      return (IActionResult) new EmptyResult();
    }

    [HttpGet("fake-token")]
    [AllowAnonymous]
    public async Task<IActionResult> FakeToken()
    {
      var token = await _tokenService.CreateNewTokenAsJsonAsync(
        new User
        {
          UserName = "czifro",
          Id = "Blah",
          SecurityStamp = Guid.NewGuid().ToString()
        }
      );
      return new OkObjectResult(token);
    }

    [HttpGet("test")]
    [LusvaAuthorize]
    public IActionResult Test()
    {
      return new OkObjectResult(this.HttpContext.User.Claims.Select(x => new { type = x.Type, val = x.Value}));
    }
  }
}