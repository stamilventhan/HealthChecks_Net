using HealthChecks.API.Common;
using HealthChecks.API.Data;
using HealthChecks.API.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Required inside Docker
});

// Add services to the container.

builder.Services.AddHealthChecks().
                AddCheck<SQLServerHealthCheck>("Sql server connectivity check", failureStatus: HealthStatus.Unhealthy);

builder.Services.AddDbContext<MyDbContext>(option => 
                                option.UseSqlServer(builder.Configuration.GetConnectionString("sqlserver")));

builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())  // Ensure it's enabled in production
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // This makes Swagger available at the root URL
    });
}
app.UseHttpsRedirection();
//HealthCheck Middleware
app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/Users", async (IConnectionFactory connectionFactory) => {

    using var connection = connectionFactory.CreateConnection("sqlserver");
    await connection.OpenAsync();
    var command = connection.CreateCommand();
    command.CommandText = "select Id, Name from Users";
    var users = new List<User>();
    var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        users.Add(new User { Id = reader.GetInt32(0), Name = reader.GetString(1) });
    }

    return Results.Ok(users);

});


app.Run();


