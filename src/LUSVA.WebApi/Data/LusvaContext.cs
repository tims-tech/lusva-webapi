using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LUSVA.WebApi.Entities;


namespace LUSVA.WebApi.Data
{
  public class LusvaContext : IdentityDbContext<User>
  {

    public LusvaContext(DbContextOptions options)
      : base(options)
    {
    }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }

    public DbSet<Event> Events { get; set; }
    
  }
}