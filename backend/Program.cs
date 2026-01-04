using CodePracticePlatform.Api.Repositories;
using CodePracticePlatform.Api.Factories;
using CodePracticePlatform.Api.Services;
using CodePracticePlatform.Api.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// File-based storage - No database needed
// Repository pattern - Register repositories for submissions and evaluations only (stored as JSON files)
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();

// Factory pattern - Register factories
builder.Services.AddScoped<IProblemFactory, ProblemFactory>();
builder.Services.AddScoped<IFeatureFactory, FeatureFactory>();

// Services - Register services
builder.Services.AddScoped<IGitService, GitService>();
builder.Services.AddScoped<ProblemFileService>(); // Loads problems from JSON files
builder.Services.AddScoped<IProblemService, ProblemService>(); // Wraps ProblemFileService
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IEvaluationService, EvaluationService>();

// Strategy pattern - Register evaluation strategies
builder.Services.AddScoped<AutomatedTestStrategy>();

// Use AutomatedTestStrategy as the default evaluation strategy
builder.Services.AddScoped<IEvaluationStrategy>(sp =>
    sp.GetRequiredService<AutomatedTestStrategy>());

// CORS configuration for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:8080", "http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

// Expose Program for WebApplicationFactory in integration tests
public partial class Program { }

