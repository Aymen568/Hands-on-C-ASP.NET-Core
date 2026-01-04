# Backend Tests

Comprehensive test suite for the Code Practice Platform backend API.

## Project Structure

```
backend.tests/
├── Models/
│   ├── ProblemModelTests.cs        - Problem model validation tests
│   ├── SubmissionModelTests.cs      - Submission model state tests
│   └── EvaluationModelTests.cs      - Evaluation scoring & feedback tests
├── Services/
│   ├── ProblemServiceTests.cs       - Problem service logic tests (mocked)
│   ├── SubmissionServiceTests.cs    - Submission service logic tests (mocked)
│   └── EvaluationServiceTests.cs    - Evaluation service logic tests (mocked)
├── Controllers/
│   ├── ProblemsControllerTests.cs   - API endpoint tests
│   ├── SubmissionsControllerTests.cs- API endpoint tests
│   └── EvaluationsControllerTests.cs- API endpoint tests
├── Integration/
│   ├── SubmissionRepositoryTests.cs - File I/O tests with real temp files
│   └── EvaluationRepositoryTests.cs - File I/O tests with real temp files
├── EdgeCases/
│   └── EdgeCaseTests.cs             - Edge cases and error scenarios
├── E2E/
│   └── SubmissionWorkflowTests.cs   - End-to-end workflow tests
└── backend.tests.csproj             - Test project configuration

```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~ProblemModelTests"
```

### Run with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Run Tests in Verbose Mode
```bash
dotnet test -v detailed
```

## Test Categories

### 1. **Unit Tests** (Models)
- Problem validation, estimated time calculation
- Submission status transitions, evaluated timestamp
- Evaluation score calculation, feedback generation
- **Coverage Target**: 95%

### 2. **Unit Tests** (Services - Mocked)
- ProblemService: Getting/filtering problems, cache clearing
- SubmissionService: CRUD operations, validation, Git URL checking
- EvaluationService: Evaluation orchestration, status updates
- **Coverage Target**: 85%

### 3. **Controller Tests** (API Layer)
- HTTP status codes (200, 201, 400, 404, 500)
- Request/Response validation
- Exception handling
- **Coverage Target**: 80%

### 4. **Integration Tests** (File I/O)
- SubmissionRepository: Create, read, update, delete operations
- EvaluationRepository: File persistence, filtering
- Concurrent file access, data integrity
- **Coverage Target**: 90%

### 5. **Edge Cases**
- Empty datasets, concurrent writes, file locking
- Invalid Git URLs, malformed JSON
- Zero total tests, invalid origins
- Status transitions

### 6. **E2E Tests**
- Complete submission workflow: Create → Evaluate → Score
- Problem filtering by difficulty/feature
- Evaluation score calculation across test cases
- Feedback generation

## Dependencies

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.2" />
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.4" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="System.IO.Abstractions" Version="21.0.2" />
<PackageReference Include="System.IO.Abstractions.TestingHelpers" Version="21.0.2" />
```

## Key Testing Patterns

### Mocking with Moq
```csharp
var mockService = new Mock<IService>();
mockService.Setup(x => x.GetAsync(1))
    .ReturnsAsync(expectedResult);
```

### FluentAssertions
```csharp
result.Should().NotBeNull();
result.Should().HaveCount(2);
result.First().Title.Should().Be("Test");
```

### Temporary File Testing
```csharp
var testDir = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}");
Directory.CreateDirectory(testDir);
// ... run tests ...
Directory.Delete(testDir, true);
```

## Test Naming Convention

```
[Method]_[Scenario]_[Expected Result]

Examples:
- Validate_WithAllValidFields_ReturnsTrue
- GetSubmissionByIdAsync_WithInvalidId_ReturnsNull
- CreateSubmission_WithValidData_Returns201
```

## Expected Test Results

| Category | Count | Status |
|----------|-------|--------|
| Model Tests | 24 | ✅ |
| Service Tests | 30 | ✅ |
| Controller Tests | 22 | ✅ |
| Integration Tests | 18 | ✅ |
| Edge Cases | 10 | ✅ |
| E2E Workflows | 7 | ✅ |
| **Total** | **~110** | **✅** |

## Coverage Goals

- **Overall**: 85%
- **Models**: 95%
- **Services**: 85%
- **Controllers**: 80%
- **Repositories**: 90%

## Continuous Integration

Tests are configured to run on:
- Pull requests
- Main branch commits
- Automated nightly builds

Minimum coverage gate: **85%**

## Troubleshooting

### Test Failures
- Check that `backend` project builds successfully
- Ensure all dependencies are installed: `dotnet restore`
- Clear build cache: `dotnet clean`

### File I/O Tests Issues
- Verify temp directory is writable
- Check for file locking issues (close IDEs if needed)
- Ensure adequate disk space

### Mock Issues
- Verify mock setup matches actual method signatures
- Check Times.Once vs Times.AtLeastOnce expectations
- Use `mockService.VerifyAll()` for strict verification

## Contributing Tests

When adding new features:
1. Write unit tests first (TDD)
2. Add integration tests for data access
3. Add controller tests for API endpoints
4. Add E2E tests for workflows
5. Maintain 85%+ coverage

## References

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions](https://fluentassertions.com/)
