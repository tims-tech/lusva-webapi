using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace LUSVA.Data
{
  public class LusvaContext : DbContext
  {

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }

    public override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      optionsBuilder.UseSqlite("Filename=./lusva.db");
    }
  }
}