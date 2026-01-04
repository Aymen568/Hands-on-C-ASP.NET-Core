using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Factories;


public interface IFeatureFactory
{
    Feature CreateFeature(string name, string description, Difficulty difficulty, FeatureType type, List<string> requiredFiles, List<string> testCases);
}

