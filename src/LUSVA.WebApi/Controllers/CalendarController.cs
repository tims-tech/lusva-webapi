using LUSVA.WebApi.Attributes;
using LUSVA.WebApi.Data;
using LUSVA.WebApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LUSVA.WebApi.Controllers
{
  [Route("api/calendar")]
  [LusvaAuthorize]
  public class CalendarController : Controller
  {
    private readonly UserManager<User> _userManager;
  }
}