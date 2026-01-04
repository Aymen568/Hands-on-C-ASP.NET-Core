using System.Net;
using System.Net.Http.Json;
using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CodePracticePlatform.Api.Tests.Integration.Api;

public class ProblemsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProblemsApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProblems_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/problems");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var problems = await response.Content.ReadFromJsonAsync<List<Problem>>();
        problems.Should().NotBeNull();
        problems!.Count.Should().BeGreaterThan(0);
    }
}

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace problem service with in-memory fake to avoid file system dependency in tests
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IProblemService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IProblemService, FakeProblemService>();

            services.Configure<MvcOptions>(options =>
            {
                options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
                options.OutputFormatters.Add(new StreamJsonOutputFormatter());
            });
        });
    }
}

internal class StreamJsonOutputFormatter : TextOutputFormatter
{
    public StreamJsonOutputFormatter()
    {
        SupportedMediaTypes.Add("application/json");
        SupportedEncodings.Add(System.Text.Encoding.UTF8);
    }

    protected override bool CanWriteType(Type? type) => true;

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, System.Text.Encoding selectedEncoding)
    {
        context.HttpContext.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(context.HttpContext.Response.Body, context.Object, context.Object?.GetType() ?? typeof(object));
    }
}

public class FakeProblemService : IProblemService
{
    private readonly List<Problem> _problems;

    public FakeProblemService()
    {
        _problems = new List<Problem>
        {
            new Problem { Id = 1, Title = "Sample Problem", Difficulty = Difficulty.Easy, FeatureType = FeatureType.Algorithm }
        };
    }

    public Task<IEnumerable<Problem>> GetProblemsAsync(Difficulty? difficulty, FeatureType? featureType)
    {
        IEnumerable<Problem> result = _problems;
        if (difficulty.HasValue)
        {
            result = result.Where(p => p.Difficulty == difficulty.Value);
        }
        if (featureType.HasValue)
        {
            result = result.Where(p => p.FeatureType == featureType.Value);
        }
        return Task.FromResult(result);
    }

    public Task<Problem?> GetProblemByIdAsync(int id) => Task.FromResult(_problems.FirstOrDefault(p => p.Id == id));
    public Task<IEnumerable<Problem>> GetAllProblemsAsync() => Task.FromResult<IEnumerable<Problem>>(_problems);
    public Task<IEnumerable<Problem>> GetProblemsByDifficultyAsync(Difficulty difficulty) => Task.FromResult<IEnumerable<Problem>>(_problems.Where(p => p.Difficulty == difficulty));
    public Task<IEnumerable<Problem>> GetProblemsByFeatureTypeAsync(FeatureType featureType) => Task.FromResult<IEnumerable<Problem>>(_problems.Where(p => p.FeatureType == featureType));
    public void ReloadProblems() { /* no-op */ }
}
