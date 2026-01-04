namespace CodePracticePlatform.Api.Models;

/// <summary>
/// Represents a missing feature that needs to be implemented in a problem
/// </summary>
public class Feature
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public FeatureType Type { get; set; }
    public List<string> RequiredFiles { get; set; } = new();
    public List<string> TestCases { get; set; } = new();
    public int ProblemId { get; set; }
    public Problem? Problem { get; set; }

    public bool ValidateRequirements()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return false;

        if (string.IsNullOrWhiteSpace(Description))
            return false;

        if (RequiredFiles == null || RequiredFiles.Count == 0)
            return false;

        return true;
    }
}

