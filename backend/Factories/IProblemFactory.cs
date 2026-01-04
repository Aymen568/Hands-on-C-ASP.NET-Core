using CodePracticePlatform.Api.Models;

namespace CodePracticePlatform.Api.Factories;


public interface IProblemFactory
{
    Problem CreateProblem(string title, string description, Difficulty difficulty, FeatureType featureType, string gitTemplateUrl);
    Problem CreateProblemFromJson(string jsonData);
}

