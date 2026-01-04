namespace CodePracticePlatform.Api.Tests.Services;

public class SubmissionServiceTests
{
    [Fact]
    public async Task GetSubmissionByIdAsync_WithValidId_ReturnsSubmission()
    {
        // Arrange
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockProblemService = new Mock<IProblemService>();
        var mockGitService = new Mock<IGitService>();
        var mockEvaluationService = new Mock<IEvaluationService>();
        var mockLogger = new Mock<ILogger<SubmissionService>>();

        var submission = new Submission { Id = 1, ProblemId = 1 };
        mockSubmissionRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(submission);

        var service = new SubmissionService(mockSubmissionRepo.Object, mockProblemService.Object, mockGitService.Object, mockEvaluationService.Object, mockLogger.Object);

        // Act
        var result = await service.GetSubmissionByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetSubmissionByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockProblemService = new Mock<IProblemService>();
        var mockGitService = new Mock<IGitService>();
        var mockEvaluationService = new Mock<IEvaluationService>();
        var mockLogger = new Mock<ILogger<SubmissionService>>();

        mockSubmissionRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Submission?)null);

        var service = new SubmissionService(mockSubmissionRepo.Object, mockProblemService.Object, mockGitService.Object, mockEvaluationService.Object, mockLogger.Object);

        // Act
        var result = await service.GetSubmissionByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllSubmissionsAsync_ReturnsAllSubmissions()
    {
        // Arrange
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockProblemService = new Mock<IProblemService>();
        var mockGitService = new Mock<IGitService>();
        var mockEvaluationService = new Mock<IEvaluationService>();
        var mockLogger = new Mock<ILogger<SubmissionService>>();

        var submissions = new List<Submission>
        {
            new Submission { Id = 1, ProblemId = 1 },
            new Submission { Id = 2, ProblemId = 2 }
        };
        mockSubmissionRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(submissions);

        var service = new SubmissionService(mockSubmissionRepo.Object, mockProblemService.Object, mockGitService.Object, mockEvaluationService.Object, mockLogger.Object);

        // Act
        var result = await service.GetAllSubmissionsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetSubmissionsByProblemIdAsync_FiltersCorrectly()
    {
        // Arrange
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockProblemService = new Mock<IProblemService>();
        var mockGitService = new Mock<IGitService>();
        var mockEvaluationService = new Mock<IEvaluationService>();
        var mockLogger = new Mock<ILogger<SubmissionService>>();

        var submissions = new List<Submission>
        {
            new Submission { Id = 1, ProblemId = 1 },
            new Submission { Id = 2, ProblemId = 1 }
        };
        mockSubmissionRepo.Setup(x => x.GetByProblemIdAsync(1)).ReturnsAsync(submissions);

        var service = new SubmissionService(mockSubmissionRepo.Object, mockProblemService.Object, mockGitService.Object, mockEvaluationService.Object, mockLogger.Object);

        // Act
        var result = await service.GetSubmissionsByProblemIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.All(s => s.ProblemId == 1).Should().BeTrue();
    }

    [Fact]
    public async Task CreateSubmissionAsync_WithValidData_CreatesSubmission()
    {
        // Arrange
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockProblemService = new Mock<IProblemService>();
        var mockGitService = new Mock<IGitService>();
        var mockEvaluationService = new Mock<IEvaluationService>();
        var mockLogger = new Mock<ILogger<SubmissionService>>();

        var problem = new Problem { Id = 1, Title = "Test" };
        var submission = new Submission { Id = 1, ProblemId = 1 };

        mockProblemService.Setup(x => x.GetProblemByIdAsync(1)).ReturnsAsync(problem);
        mockGitService.Setup(x => x.ValidateRepositoryUrlAsync("https://github.com/test/repo")).ReturnsAsync(true);
        mockSubmissionRepo.Setup(x => x.CreateAsync(It.IsAny<Submission>())).ReturnsAsync(submission);

        var service = new SubmissionService(mockSubmissionRepo.Object, mockProblemService.Object, mockGitService.Object, mockEvaluationService.Object, mockLogger.Object);

        // Act
        var result = await service.CreateSubmissionAsync(1, 1, "https://github.com/test/repo");

        // Assert
        result.Should().NotBeNull();
        result.ProblemId.Should().Be(1);
        mockSubmissionRepo.Verify(x => x.CreateAsync(It.IsAny<Submission>()), Times.Once);
    }
}
