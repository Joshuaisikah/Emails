using System.Threading.Channels;
using EmailService.Library.Model;
using EmailService.Library.Persistence;
using EmailService.Library.Repositories;
using EmailService.Library.Services;
using EmailService.Library.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure EmailConfig from appsettings.json
var emailConfig = builder.Configuration
    .GetSection("EmailConfig")
    .Get<EmailConfig>() ?? throw new InvalidOperationException("EmailConfig section is missing or invalid.");
builder.Services.AddSingleton(emailConfig);

// Register services
builder.Services.AddControllers();
//builder.Services.AddDbContext<EmailDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Configure SQLite database
builder.Services.AddDbContext<EmailDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register in-memory channel for background processing
builder.Services.AddSingleton(Channel.CreateUnbounded<Email>());

// Dependency Injection
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<ISendEmailService, SendEmailService>();
builder.Services.AddHostedService<EmailBackgroundService>();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailService.API", Version = "v1" });
});

var app = builder.Build();

// HTTP request pipeline setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
