using System.Text.Json.Serialization;

namespace FluentValidationCLI.Generator.Models;

public class RuleDefinition
{
    [JsonPropertyName("entityName")]
    public string EntityName { get; set; } = string.Empty;

    [JsonPropertyName("properties")]
    public List<PropertyDefinition> Properties { get; set; } = new();
}

public class PropertyDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("rules")]
    public List<RuleItem> Rules { get; set; } = new();
}

public class RuleItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("min")]
    public int? Min { get; set; }

    [JsonPropertyName("max")]
    public int? Max { get; set; }

    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }
}
