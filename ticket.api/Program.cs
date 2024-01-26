using core.data.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ticket.api.Repository;
using ticket.api.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDataPersistence(builder.Configuration.GetConnectionString("MicroserviceDb"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TicketRepository>();
builder.Services.AddTransient<TicketService>();
builder.Services.AddHttpClient<DocumentService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("DocumentServiceAgent"));
});
builder.Services.AddHttpLogging(options => { });
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context => new UnprocessableEntityObjectResult(context.ModelState);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration.GetValue<string>("AuthProvider");
    options.Audience = "ticket";
    options.RequireHttpsMetadata = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
