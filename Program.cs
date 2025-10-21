using Microsoft.EntityFrameworkCore;
using PerformerApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")
              ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connStr, npgsql =>
    {
        npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }));

var corsPolicy = "_allowFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://performance-review-panel-frontend-o.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
