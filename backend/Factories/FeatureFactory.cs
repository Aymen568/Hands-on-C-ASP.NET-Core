using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Factories;

public class FeatureFactory : IFeatureFactory
{
    public Feature CreateFeature(string name, string description, Difficulty difficulty, FeatureType type, List<string> requiredFiles, List<string> testCases)
    {
        return new Feature
        {
            Name = name,
            Description = description,
            Difficulty = difficulty,
            Type = type,
            RequiredFiles = requiredFiles ?? new List<string>(),
            TestCases = testCases ?? new List<string>()
        };
    }
}

