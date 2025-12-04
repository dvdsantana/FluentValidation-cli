using System.Text;
using FluentValidation.Generator.Mapping;
using FluentValidation.Generator.Models;

namespace FluentValidation.Generator.Services;

/// <summary>
/// Generates C# FluentValidation validator classes from ValidationDefinitions
/// </summary>
public class CSharpCodeGenerator
{
    private const int IndentSize = 4;

    /// <summary>
    /// Generates C# validator code for a single entity
    /// </summary>
    /// <param name="definition">The validation definition to generate code from</param>
    /// <returns>Generated C# code as a string</returns>
    public string GenerateValidator(ValidationDefinition definition)
    {
        var sb = new StringBuilder();

        // Add using statements
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine();

        // Add namespace
        sb.AppendLine($"namespace {definition.Namespace}");
        sb.AppendLine("{");

        // Add validator class
        var className = $"{definition.Entity}Validator";
        sb.AppendLine(
            $"{Indent(1)}public class {className} : AbstractValidator<{definition.Entity}>"
        );
        sb.AppendLine($"{Indent(1)}{{");

        // Add constructor
        sb.AppendLine($"{Indent(2)}public {className}()");
        sb.AppendLine($"{Indent(2)}{{");

        // Generate rules for each property
        foreach (var property in definition.Properties)
        {
            GeneratePropertyRules(sb, property);
        }

        // Close constructor
        sb.AppendLine($"{Indent(2)}}}");

        // Close class
        sb.AppendLine($"{Indent(1)}}}");

        // Close namespace
        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Generates validation rules for a single property
    /// </summary>
    private void GeneratePropertyRules(StringBuilder sb, PropertyValidation property)
    {
        if (property.Rules.Count == 0)
            return;

        // Start RuleFor chain
        sb.AppendLine();
        sb.Append($"{Indent(3)}RuleFor(x => x.{property.Name})");

        // Add each validation rule in the chain
        for (int i = 0; i < property.Rules.Count; i++)
        {
            var rule = property.Rules[i];
            sb.AppendLine();
            sb.Append(
                $"{Indent(4)}.{ValidatorMapping.MapToCSharp(rule.Validator, rule.Parameters)}"
            );

            // Add custom message if provided
            if (!string.IsNullOrWhiteSpace(rule.Message))
            {
                sb.AppendLine();
                var escapedMessage = EscapeString(rule.Message);
                sb.Append($"{Indent(4)}.WithMessage(\"{escapedMessage}\")");
            }
        }

        // End RuleFor statement
        sb.AppendLine(";");
    }

    /// <summary>
    /// Creates indentation string
    /// </summary>
    private string Indent(int level)
    {
        return new string(' ', level * IndentSize);
    }

    /// <summary>
    /// Escapes special characters in strings for C# code
    /// </summary>
    private string EscapeString(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    /// <summary>
    /// Generates the filename for the validator
    /// </summary>
    public string GetValidatorFileName(ValidationDefinition definition)
    {
        return $"{definition.Entity}Validator.cs";
    }
}
