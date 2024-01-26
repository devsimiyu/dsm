using core.data.Persistence;
using identity.api.Repository;
using identity.api.Service;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDataPersistence(builder.Configuration.GetConnectionString("MicroserviceDb"));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<OpenIdConnectService>();
builder.Services.AddControllers();
builder.Services.AddHttpLogging(options => { });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpLogging();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
