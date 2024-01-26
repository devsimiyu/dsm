using document.api.Repository;
using document.api.Service;
using core.data.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDataPersistence(builder.Configuration.GetConnectionString("MicroservicesDb"));
builder.Services.AddScoped<DocumentRepository>();
builder.Services.AddTransient<DocumentService>();
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context => new UnprocessableEntityObjectResult(context.ModelState);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration.GetValue<string>("AuthProvider");
    options.Audience = "document";
    options.RequireHttpsMetadata = false;
});
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
