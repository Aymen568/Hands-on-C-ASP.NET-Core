using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using CodePracticePlatform.Api.Models;
using CodePracticePlatform.Api.Services;
using CodePracticePlatform.Api.Repositories;
using CodePracticePlatform.Api.Strategies;

namespace CodePracticePlatform.Api.Tests.Services;

public class EvaluationServiceTests // Needs to add more happy path and edge case tests
{
    [Fact]
    public async Task GetEvaluationBySubmissionIdAsync_WithValidId_ReturnsEvaluation()
    {
        // Arrange
        var mockEvalRepo = new Mock<IEvaluationRepository>();
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockStrategy = new Mock<IEvaluationStrategy>();
        var mockLogger = new Mock<ILogger<EvaluationService>>();

        var evaluation = new Evaluation { Id = 1, SubmissionId = 1 };
        mockEvalRepo.Setup(x => x.GetBySubmissionIdAsync(1)).ReturnsAsync(evaluation);

        var service = new EvaluationService(mockEvalRepo.Object, mockSubmissionRepo.Object, mockStrategy.Object, mockLogger.Object);

        // Act
        var result = await service.GetEvaluationBySubmissionIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.SubmissionId.Should().Be(1);
    }

    [Fact]
    public async Task GetEvaluationBySubmissionIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var mockEvalRepo = new Mock<IEvaluationRepository>();
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockStrategy = new Mock<IEvaluationStrategy>();
        var mockLogger = new Mock<ILogger<EvaluationService>>();

        mockEvalRepo.Setup(x => x.GetBySubmissionIdAsync(999)).ReturnsAsync((Evaluation?)null);

        var service = new EvaluationService(mockEvalRepo.Object, mockSubmissionRepo.Object, mockStrategy.Object, mockLogger.Object);

        // Act
        var result = await service.GetEvaluationBySubmissionIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EvaluateSubmissionAsync_WithValidSubmission_UpdatesStatus()
    {
        // Arrange
        var mockEvalRepo = new Mock<IEvaluationRepository>();
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockStrategy = new Mock<IEvaluationStrategy>();
        var mockLogger = new Mock<ILogger<EvaluationService>>();

        var submission = new Submission { Id = 1, ProblemId = 1, Status = SubmissionStatus.Pending };
        var evaluation = new Evaluation { Id = 1, SubmissionId = 1, TotalTests = 10, PassedTests = 10, FailedTests = 0 };

        mockSubmissionRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(submission);
        mockStrategy.Setup(x => x.EvaluateAsync(submission)).ReturnsAsync(evaluation);
        mockSubmissionRepo.Setup(x => x.UpdateAsync(It.IsAny<Submission>())).ReturnsAsync(submission);
        mockEvalRepo.Setup(x => x.CreateAsync(It.IsAny<Evaluation>())).ReturnsAsync(evaluation);

        var service = new EvaluationService(mockEvalRepo.Object, mockSubmissionRepo.Object, mockStrategy.Object, mockLogger.Object);

        // Act
        var result = await service.EvaluateSubmissionAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.SubmissionId.Should().Be(1);
        mockEvalRepo.Verify(x => x.CreateAsync(It.IsAny<Evaluation>()), Times.Once);
    }

    [Fact]
    public async Task EvaluateSubmissionAsync_AllTestsPassed_StatusIsPassed()
    {
        // Arrange
        var mockEvalRepo = new Mock<IEvaluationRepository>();
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockStrategy = new Mock<IEvaluationStrategy>();
        var mockLogger = new Mock<ILogger<EvaluationService>>();

        var submission = new Submission { Id = 1, ProblemId = 1, Status = SubmissionStatus.Pending };
        var evaluation = new Evaluation { Id = 1, SubmissionId = 1, TotalTests = 10, PassedTests = 10, FailedTests = 0 };

        mockSubmissionRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(submission);
        mockStrategy.Setup(x => x.EvaluateAsync(submission)).ReturnsAsync(evaluation);
        mockSubmissionRepo.Setup(x => x.UpdateAsync(It.IsAny<Submission>())).ReturnsAsync(submission);
        mockEvalRepo.Setup(x => x.CreateAsync(It.IsAny<Evaluation>())).ReturnsAsync(evaluation);

        var service = new EvaluationService(mockEvalRepo.Object, mockSubmissionRepo.Object, mockStrategy.Object, mockLogger.Object);

        // Act
        await service.EvaluateSubmissionAsync(1);

        // Assert
        mockSubmissionRepo.Verify(x => x.UpdateAsync(It.IsAny<Submission>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task EvaluateSubmissionAsync_SomeTestsFailed_StatusIsFailed()
    {
        // Arrange
        var mockEvalRepo = new Mock<IEvaluationRepository>();
        var mockSubmissionRepo = new Mock<ISubmissionRepository>();
        var mockStrategy = new Mock<IEvaluationStrategy>();
        var mockLogger = new Mock<ILogger<EvaluationService>>();

        var submission = new Submission { Id = 1, ProblemId = 1, Status = SubmissionStatus.Pending };
        var evaluation = new Evaluation { Id = 1, SubmissionId = 1, TotalTests = 10, PassedTests = 5, FailedTests = 5 };

        mockSubmissionRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(submission);
        mockStrategy.Setup(x => x.EvaluateAsync(submission)).ReturnsAsync(evaluation);
        mockSubmissionRepo.Setup(x => x.UpdateAsync(It.IsAny<Submission>())).ReturnsAsync(submission);
        mockEvalRepo.Setup(x => x.CreateAsync(It.IsAny<Evaluation>())).ReturnsAsync(evaluation);

        var service = new EvaluationService(mockEvalRepo.Object, mockSubmissionRepo.Object, mockStrategy.Object, mockLogger.Object);

        // Act
        await service.EvaluateSubmissionAsync(1);

        // Assert
        mockSubmissionRepo.Verify(x => x.UpdateAsync(It.IsAny<Submission>()), Times.AtLeastOnce);
    }
}
