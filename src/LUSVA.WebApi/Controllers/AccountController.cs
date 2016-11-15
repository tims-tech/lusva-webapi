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
    public async Task<IActionResult> SignIn([FromBody] SignInModel model)
    {
      var userTask = _userManager.FindByEmailAsync(model.Email);
      var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);

      if (!result.Succeeded)
        return new BadRequestResult();

      var user = await userTask;

      if (user == null)
        return new BadRequestResult();

      var token = await _tokenService.CreateNewTokenAsJsonAsync(user);

      return new OkObjectResult(token);
    }

    [HttpPost("sign-out")]
    [LusvaAuthorize]
    public async Task<IActionResult> SignOut()
    {
      var user = await _userManager.GetUserAsync(User);
      await _signInManager.SignOutAsync();
      await _userManager.UpdateSecurityStampAsync(user);
      return new OkResult();
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
      var user = await _userManager.FindByEmailAsync(model.Email);

      IdentityResult result;

      if (user != null && !user.IsInvited)
        return new BadRequestResult();
      else
        user = new User();

      user.FirstName = model.FirstName;
      user.LastName = model.LastName;
      user.UserName = model.Email.Substring(0, model.Email.IndexOf('@')).Trim();

      if (string.IsNullOrEmpty(user.SecurityStamp))
      {
        await _userManager.AddPasswordAsync(user, model.Password);
        if (!(await _userManager.UpdateAsync(user)).Succeeded)
          return new BadRequestResult();
      }
      else
      {
        if (!(await _userManager.CreateAsync(user, model.Password)).Succeeded)
          return new BadRequestResult();
      }

      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      return new OkResult();
    }

    [HttpPost("invite")]
    [LusvaAuthorize]
    public async Task<IActionResult> Invite(InviteNewUserModel model)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user != null)
        return new OkObjectResult(new { message = user.IsInvited ? "Invite pending" : "User exists" });
      user = new User
      {
        UserName = model.Email.Substring(0, model.Email.IndexOf('@')).Trim(),
        Email = model.Email,
        IsInvited = true
      };
      await _userManager.CreateAsync(user);
      return new OkObjectResult(new { message = "Invite sent" });
    }

    [HttpGet("fake-token")]
    [AllowAnonymous]
    public async Task<IActionResult> FakeToken()
    {
      var token = await _tokenService.CreateNewTokenAsJsonAsync(
        new User
        {
          Email = "czifro@gmail.com",
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
      var res = new {name = User.Identity.Name, id = _userManager.GetUserId(User)};
      return new OkObjectResult(res); //User.Identity.Name);
      //.Claims.Select(x => new { type = x.Type, val = x.Value}));
    }
  }
}