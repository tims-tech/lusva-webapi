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


    public List<Event> EventsAPartOf { get; set; }
  } 
}