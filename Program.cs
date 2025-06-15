using EmailRegistroAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- START: Add services to the container ---

// 1. Configure DbContext with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Add services for dependency injection
builder.Services.AddScoped<IEmailService, EmailService>();

// 3. Add in-memory cache for temporary storage of verification codes
// NOTE: For production, consider a distributed cache like Redis.
builder.Services.AddMemoryCache();

// --- END: Add services to the container ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();