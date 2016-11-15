using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LUSVA.WebApi.Entities
{
  public class User : IdentityUser
  {
    // Identity Framework handles other fields

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsInvited { get; set; }

    public List<Event> EventsAPartOf { get; set; }
  } 
}