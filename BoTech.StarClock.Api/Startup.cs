using Microsoft.OpenApi.Models;

namespace BoTech.StarClock.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add support for controllers
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Internal Api of the BoTechStarClock",
                Version = "v1.1",
                Description = "This Api can be used by clients to control the StarClock (Homepod) remotely",
                Contact = new OpenApiContact
                {
                    Name = "BoTech (Florian T.)",
                    Email = "support@botech.dev",
                    Url = new Uri("https://aka.botech.dev/bot.sc")
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at the root (optional)
        });

        //app.UseHttpsRedirection();
        
        // Optional: Developer exception page
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Routing and endpoints
        app.UseRouting();
        
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Maps [ApiController] endpoints
        });
    }
}
