using System.Text;
using System.Text.Json;
using FluentValidationCLI.Generator.Models;

namespace FluentValidationCLI.Generator.Generators;

public class TypeScriptGenerator
{
    public string Generate(RuleDefinition ruleDef)
    {
        var sb = new StringBuilder();
        sb.AppendLine("import { Validator } from 'fluentvalidation-ts';");
        sb.AppendLine();

        // Generate Interface
        sb.AppendLine($"export interface {ruleDef.EntityName} {{");
        foreach (var prop in ruleDef.Properties)
        {
            string tsType = prop.Type switch
            {
                "string" => "string",
                "number" => "number",
                "boolean" => "boolean",
                _ => "any",
            };
            sb.AppendLine($"  {prop.Name.ToLower()}: {tsType};"); // Assuming camelCase for TS properties
        }
        sb.AppendLine("}");
        sb.AppendLine();

        // Generate Validator
        sb.AppendLine(
            $"export class {ruleDef.EntityName}Validator extends Validator<{ruleDef.EntityName}> {{"
        );
        sb.AppendLine("  constructor() {");
        sb.AppendLine("    super();");
        sb.AppendLine();

        foreach (var prop in ruleDef.Properties)
        {
            sb.Append($"    this.ruleFor('{prop.Name.ToLower()}')");

            foreach (var rule in prop.Rules)
            {
                AppendRule(sb, rule);
            }
            sb.AppendLine(";");
        }

        sb.AppendLine("  }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private void AppendRule(StringBuilder sb, RuleItem rule)
    {
        switch (rule.Type)
        {
            case "NotEmpty":
                sb.Append(".notEmpty()");
                break;
            case "Length":
                if (rule.Min.HasValue && rule.Max.HasValue)
                    sb.Append($".length({rule.Min}, {rule.Max})");
                else if (rule.Min.HasValue)
                    sb.Append($".minLength({rule.Min})");
                else if (rule.Max.HasValue)
                    sb.Append($".maxLength({rule.Max})");
                break;
            case "EmailAddress":
                sb.Append(".emailAddress()");
                break;
            case "GreaterThan":
                sb.Append($".greaterThan({GetValue(rule.Value)})");
                break;
            case "LessThan":
                sb.Append($".lessThan({GetValue(rule.Value)})");
                break;
            case "Matches":
                // Regex needs handling, assuming string pattern
                if (!string.IsNullOrEmpty(rule.Pattern))
                    sb.Append($".matches(/{rule.Pattern}/)");
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(rule.Message))
        {
            sb.Append($".withMessage('{rule.Message}')");
        }
    }

    private string GetValue(object? value)
    {
        if (value is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String)
                return $"'{element.GetString()}'";
            return element.ToString();
        }
        return value?.ToString() ?? "null";
    }
}
