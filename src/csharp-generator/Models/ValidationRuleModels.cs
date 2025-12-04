namespace FluentValidation.Generator.Models;

/// <summary>
/// Root model representing a complete validation definition for an entity
/// </summary>
public class ValidationDefinition
{
    /// <summary>
    /// Name of the entity being validated (e.g., "User", "Product")
    /// </summary>
    public string Entity { get; set; } = string.Empty;

    /// <summary>
    /// Namespace for the generated C# validator class
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// List of properties with their validation rules
    /// </summary>
    public List<PropertyValidation> Properties { get; set; } = new();
}

/// <summary>
/// Validation definition for a single property
/// </summary>
public class PropertyValidation
{
    /// <summary>
    /// Name of the property (e.g., "Email", "Age")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Data type of the property (string, number, boolean, date)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// List of validation rules to apply to this property
    /// </summary>
    public List<ValidationRule> Rules { get; set; } = new();
}

/// <summary>
/// Individual validation rule
/// </summary>
public class ValidationRule
{
    /// <summary>
    /// Name of the validator (e.g., "NotEmpty", "EmailAddress", "Length")
    /// </summary>
    public string Validator { get; set; } = string.Empty;

    /// <summary>
    /// Parameters for the validator (e.g., min/max for range validators)
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// Custom error message for this validation rule
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Conditional expression for when this rule should apply (future feature)
    /// </summary>
    public string? When { get; set; }
}
