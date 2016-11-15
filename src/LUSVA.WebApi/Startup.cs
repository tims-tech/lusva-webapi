using System;
using LUSVA.WebApi.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LUSVA.WebApi.Entities;
using LUSVA.WebApi.Data;
using LUSVA.WebApi.Extensions;
using LUSVA.WebApi.Providers;
using LUSVA.WebApi.Services;

//using SimpleTokenProvider;

namespace LUSVA.WebApi
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddOptions();

      services.AddDbContext<LusvaContext>(options =>
          options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

      // Add authentication services
      services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<LusvaContext>()
        .AddDefaultTokenProviders();

      TokenServiceConfiguration.Key = "super_secret_123";

      // Setup Dependencies
      services.AddTransient<IEmailSender, MessageService>();
      services.AddScoped<ITokenService, TokenService>(provider =>
          new TokenService(TokenServiceConfiguration.DebugConfiguration));

      // Add framework services.
      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      if (env.IsDevelopment())
      {
        Console.WriteLine("Is Development");
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
        app.UseBrowserLink();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseIdentity();

      app.UseLusvaJwtBearerAuthenticationDebug(TokenServiceConfiguration.Key);

      app.UseMvc();
    }
  }
}
