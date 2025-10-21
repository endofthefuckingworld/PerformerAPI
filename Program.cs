using Microsoft.EntityFrameworkCore;
using PerformerApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 讀取連線字串（Railway/雲端優先用環境變數）
var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")
              ?? builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connStr, npgsql => npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

// ✅ 一定要註冊 Controllers（否則就會出現「Unable to find the required services」）
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS（允許本機與你的 Vercel 網域）
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

// 啟用 CORS（要在 MapControllers 之前）
app.UseCors(corsPolicy);

// Swagger（Production 也開，方便 demo）
app.UseSwagger();
app.UseSwaggerUI();

// 映射 Controllers（少了這行也會 404）
app.MapControllers();

app.Run();
