using System.Text.Json;
using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Factories;

public class ProblemFactory : IProblemFactory
{
    public Problem CreateProblem(string title, string description, Difficulty difficulty, FeatureType featureType, string gitTemplateUrl)
    {
        return new Problem
        {
            Title = title,
            Description = description,
            Difficulty = difficulty,
            FeatureType = featureType,
            GitTemplateUrl = gitTemplateUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public Problem CreateProblemFromJson(string jsonData)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var problemDto = JsonSerializer.Deserialize<ProblemDto>(jsonData, options);
        
        if (problemDto == null)
            throw new ArgumentException("Invalid JSON data for Problem creation");

        var problem = CreateProblem(
            problemDto.Title,
            problemDto.Description,
            Enum.Parse<Difficulty>(problemDto.Difficulty),
            Enum.Parse<FeatureType>(problemDto.FeatureType),
            problemDto.GitTemplateUrl
        );

        // Add features if provided
        if (problemDto.Features != null && problemDto.Features.Any())
        {
            var featureFactory = new FeatureFactory();
            foreach (var featureDto in problemDto.Features)
            {
                var feature = featureFactory.CreateFeature(
                    featureDto.Name,
                    featureDto.Description,
                    Enum.Parse<Difficulty>(featureDto.Difficulty),
                    Enum.Parse<FeatureType>(featureDto.Type),
                    featureDto.RequiredFiles ?? new List<string>(),
                    featureDto.TestCases ?? new List<string>()
                );
                problem.Features.Add(feature);
            }
        }

        return problem;
    }

    // DTO for JSON deserialization
    private class ProblemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string FeatureType { get; set; } = string.Empty;
        public string GitTemplateUrl { get; set; } = string.Empty;
        public List<FeatureDto>? Features { get; set; }
    }

    private class FeatureDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string>? RequiredFiles { get; set; }
        public List<string>? TestCases { get; set; }
    }
}

