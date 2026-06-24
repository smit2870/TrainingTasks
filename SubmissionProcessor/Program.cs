using SubmissionProcessor;
using SubmissionProcessor.Services;
using taskmanagement.Data;
using taskmanagement.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using taskmanagement.Options;

var builder = Host.CreateApplicationBuilder(args);

Env.Load();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPassword};";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<ISubmissionProcessingService, SubmissionProcessingService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();