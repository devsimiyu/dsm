using domain.data.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ticket.api.Middleware;
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
    client.BaseAddress = new Uri(builder.Configuration["DocumentServiceAgent"]);
})
.AddPolicyHandler(PollyMiddleware.GetRetryPolicy())
.AddPolicyHandler(PollyMiddleware.GetCircuitBreakerPolicy());
builder.Services.AddHttpLogging(options => { });
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration.GetValue<string>("AuthProvider");
    options.Audience = "ticket";
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
