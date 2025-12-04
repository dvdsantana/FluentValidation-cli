namespace FluentValidation.Generator.Mapping;

/// <summary>
/// Maps JSON validator names to C# FluentValidation method calls
/// </summary>
public static class ValidatorMapping
{
    /// <summary>
    /// Generates the C# FluentValidation method call for a given validator and its parameters
    /// </summary>
    /// <param name="validatorName">Name of the validator (e.g., "NotEmpty", "Length")</param>
    /// <param name="parameters">Optional parameters for the validator</param>
    /// <returns>C# method call string (e.g., "NotEmpty()", "Length(1, 100)")</returns>
    public static string MapToCSharp(string validatorName, Dictionary<string, object>? parameters)
    {
        return validatorName switch
        {
            "NotNull" => "NotNull()",
            "NotEmpty" => "NotEmpty()",
            "Empty" => "Empty()",
            "Null" => "Null()",

            "Equal" => GenerateEqual(parameters),
            "NotEqual" => GenerateNotEqual(parameters),

            "Length" => GenerateLength(parameters),
            "MinLength" => GenerateMinLength(parameters),
            "MaxLength" => GenerateMaxLength(parameters),

            "EmailAddress" => "EmailAddress()",
            "CreditCard" => "CreditCard()",

            "Matches" => GenerateMatches(parameters),

            "LessThan" => GenerateLessThan(parameters),
            "LessThanOrEqualTo" => GenerateLessThanOrEqualTo(parameters),
            "GreaterThan" => GenerateGreaterThan(parameters),
            "GreaterThanOrEqualTo" => GenerateGreaterThanOrEqualTo(parameters),

            "InclusiveBetween" => GenerateInclusiveBetween(parameters),
            "ExclusiveBetween" => GenerateExclusiveBetween(parameters),

            "IsInEnum" => "IsInEnum()",

            _ => throw new NotSupportedException($"Validator '{validatorName}' is not supported"),
        };
    }

    private static string GenerateEqual(Dictionary<string, object>? parameters)
    {
        var value = GetRequiredParameter(parameters, "value", "Equal");
        return $"Equal({FormatValue(value)})";
    }

    private static string GenerateNotEqual(Dictionary<string, object>? parameters)
    {
        var value = GetRequiredParameter(parameters, "value", "NotEqual");
        return $"NotEqual({FormatValue(value)})";
    }

    private static string GenerateLength(Dictionary<string, object>? parameters)
    {
        var min = GetRequiredParameter(parameters, "min", "Length");
        var max = GetRequiredParameter(parameters, "max", "Length");
        return $"Length({min}, {max})";
    }

    private static string GenerateMinLength(Dictionary<string, object>? parameters)
    {
        var length = GetRequiredParameter(parameters, "length", "MinLength");
        return $"MinimumLength({length})";
    }

    private static string GenerateMaxLength(Dictionary<string, object>? parameters)
    {
        var length = GetRequiredParameter(parameters, "length", "MaxLength");
        return $"MaximumLength({length})";
    }

    private static string GenerateMatches(Dictionary<string, object>? parameters)
    {
        var pattern = GetRequiredParameter(parameters, "pattern", "Matches");
        // Escape special characters in the pattern string
        var escapedPattern = pattern.ToString()?.Replace("\\", "\\\\").Replace("\"", "\\\"");
        return $"Matches(\"{escapedPattern}\")";
    }

    private static string GenerateLessThan(Dictionary<string, object>? parameters)
    {
        var value = GetRequiredParameter(parameters, "value", "LessThan");
        return $"LessThan({value})";
    }

    private static string GenerateLessThanOrEqualTo(Dictionary<string, object>? parameters)
    {
        var value = GetRequiredParameter(parameters, "value", "LessThanOrEqualTo");
        return $"LessThanOrEqualTo({value})";
    }

    private static string GenerateGreaterThan(Dictionary<string, object>? parameters)
    {
        var value = GetRequiredParameter(parameters, "value", "GreaterThan");
        return $"GreaterThan({value})";
    }

    private static string GenerateGreaterThanOrEqualTo(Dictionary<string, object>? parameters)
    {
        var value = GetRequiredParameter(parameters, "value", "GreaterThanOrEqualTo");
        return $"GreaterThanOrEqualTo({value})";
    }

    private static string GenerateInclusiveBetween(Dictionary<string, object>? parameters)
    {
        var min = GetRequiredParameter(parameters, "min", "InclusiveBetween");
        var max = GetRequiredParameter(parameters, "max", "InclusiveBetween");
        return $"InclusiveBetween({min}, {max})";
    }

    private static string GenerateExclusiveBetween(Dictionary<string, object>? parameters)
    {
        var min = GetRequiredParameter(parameters, "min", "ExclusiveBetween");
        var max = GetRequiredParameter(parameters, "max", "ExclusiveBetween");
        return $"ExclusiveBetween({min}, {max})";
    }

    private static object GetRequiredParameter(
        Dictionary<string, object>? parameters,
        string paramName,
        string validatorName
    )
    {
        if (parameters == null || !parameters.ContainsKey(paramName))
        {
            throw new InvalidOperationException(
                $"Validator '{validatorName}' requires parameter '{paramName}'"
            );
        }

        return parameters[paramName];
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            string s => $"\"{s}\"",
            bool b => b.ToString().ToLower(),
            _ => value.ToString() ?? "null",
        };
    }
}
