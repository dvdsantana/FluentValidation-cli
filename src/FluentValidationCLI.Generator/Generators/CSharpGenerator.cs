using System.Text;
using System.Text.Json;
using FluentValidationCLI.Generator.Models;

namespace FluentValidationCLI.Generator.Generators;

public class CSharpGenerator
{
    public string Generate(RuleDefinition ruleDef, string namespaceName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine("using FluentValidationCLI.Backend.Models;");
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine(
            $"public class {ruleDef.EntityName}Validator : AbstractValidator<{ruleDef.EntityName}>"
        );
        sb.AppendLine("{");
        sb.AppendLine($"    public {ruleDef.EntityName}Validator()");
        sb.AppendLine("    {");

        foreach (var prop in ruleDef.Properties)
        {
            sb.Append($"        RuleFor(x => x.{prop.Name})");

            foreach (var rule in prop.Rules)
            {
                AppendRule(sb, rule);
            }
            sb.AppendLine(";");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private void AppendRule(StringBuilder sb, RuleItem rule)
    {
        switch (rule.Type)
        {
            case "NotEmpty":
                sb.Append(".NotEmpty()");
                break;
            case "Length":
                if (rule.Min.HasValue && rule.Max.HasValue)
                    sb.Append($".Length({rule.Min}, {rule.Max})");
                else if (rule.Min.HasValue)
                    sb.Append($".MinimumLength({rule.Min})");
                else if (rule.Max.HasValue)
                    sb.Append($".MaximumLength({rule.Max})");
                break;
            case "EmailAddress":
                sb.Append(".EmailAddress()");
                break;
            case "GreaterThan":
                sb.Append($".GreaterThan({GetValue(rule.Value)})");
                break;
            case "LessThan":
                sb.Append($".LessThan({GetValue(rule.Value)})");
                break;
            // Add more rules as needed
            default:
                // Warning or ignore?
                break;
        }

        if (!string.IsNullOrEmpty(rule.Message))
        {
            sb.Append($".WithMessage(\"{rule.Message}\")");
        }
    }

    private string GetValue(object? value)
    {
        if (value is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String)
                return $"\"{element.GetString()}\"";
            return element.ToString();
        }
        return value?.ToString() ?? "null";
    }
}
