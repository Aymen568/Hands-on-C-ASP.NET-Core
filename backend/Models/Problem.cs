namespace CodePracticePlatform.Api.Models;


public class Problem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public FeatureType FeatureType { get; set; }
    public string GitTemplateUrl { get; set; } = string.Empty;
    public List<Feature> Features { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validates the problem data
    /// </summary>
    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return false;

        if (string.IsNullOrWhiteSpace(Description))
            return false;

        if (string.IsNullOrWhiteSpace(GitTemplateUrl))
            return false;

        if (!Uri.IsWellFormedUriString(GitTemplateUrl, UriKind.Absolute))
            return false;

        return true;
    }

    public TimeSpan GetEstimatedTime()
    {
        return Difficulty switch
        {
            Difficulty.Easy => TimeSpan.FromMinutes(30),
            Difficulty.Medium => TimeSpan.FromHours(2),
            Difficulty.Hard => TimeSpan.FromHours(4),
            _ => TimeSpan.FromHours(1)
        };
    }
}

