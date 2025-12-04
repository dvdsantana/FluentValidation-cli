using System.Text.Json;
using FluentValidation.Generator.Models;

namespace FluentValidation.Generator.Services;

/// <summary>
/// Service for parsing and validating JSON rule definition files
/// </summary>
public class JsonParser
{
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonParser()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };
    }

    /// <summary>
    /// Reads and parses all JSON files from the specified directory
    /// </summary>
    /// <param name="inputPath">Path to directory containing JSON rule files</param>
    /// <returns>List of parsed validation definitions</returns>
    public async Task<List<ValidationDefinition>> ParseDirectoryAsync(string inputPath)
    {
        if (!Directory.Exists(inputPath))
        {
            throw new DirectoryNotFoundException($"Input directory not found: {inputPath}");
        }

        var jsonFiles = Directory.GetFiles(inputPath, "*.json", SearchOption.TopDirectoryOnly);

        if (jsonFiles.Length == 0)
        {
            Console.WriteLine($"Warning: No JSON files found in {inputPath}");
            return new List<ValidationDefinition>();
        }

        var definitions = new List<ValidationDefinition>();

        foreach (var filePath in jsonFiles)
        {
            try
            {
                var definition = await ParseFileAsync(filePath);
                definitions.Add(definition);
                Console.WriteLine($"✓ Parsed {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error parsing {Path.GetFileName(filePath)}: {ex.Message}");
                throw;
            }
        }

        return definitions;
    }

    /// <summary>
    /// Parses a single JSON file into a ValidationDefinition
    /// </summary>
    /// <param name="filePath">Path to the JSON file</param>
    /// <returns>Parsed validation definition</returns>
    public async Task<ValidationDefinition> ParseFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"JSON file not found: {filePath}");
        }

        var jsonContent = await File.ReadAllTextAsync(filePath);

        var definition = JsonSerializer.Deserialize<ValidationDefinition>(
            jsonContent,
            _jsonOptions
        );

        if (definition == null)
        {
            throw new InvalidOperationException($"Failed to deserialize {filePath}");
        }

        ValidateDefinition(definition, filePath);

        return definition;
    }

    /// <summary>
    /// Validates that the parsed definition has all required fields
    /// </summary>
    private void ValidateDefinition(ValidationDefinition definition, string filePath)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Entity))
        {
            errors.Add("Entity name is required");
        }

        if (string.IsNullOrWhiteSpace(definition.Namespace))
        {
            errors.Add("Namespace is required");
        }

        if (definition.Properties == null || definition.Properties.Count == 0)
        {
            errors.Add("At least one property must be defined");
        }
        else
        {
            for (int i = 0; i < definition.Properties.Count; i++)
            {
                var prop = definition.Properties[i];

                if (string.IsNullOrWhiteSpace(prop.Name))
                {
                    errors.Add($"Property[{i}]: Name is required");
                }

                if (string.IsNullOrWhiteSpace(prop.Type))
                {
                    errors.Add($"Property[{i}] ({prop.Name}): Type is required");
                }

                if (prop.Rules == null || prop.Rules.Count == 0)
                {
                    errors.Add(
                        $"Property[{i}] ({prop.Name}): At least one validation rule is required"
                    );
                }
                else
                {
                    for (int j = 0; j < prop.Rules.Count; j++)
                    {
                        var rule = prop.Rules[j];
                        if (string.IsNullOrWhiteSpace(rule.Validator))
                        {
                            errors.Add(
                                $"Property[{i}] ({prop.Name}), Rule[{j}]: Validator name is required"
                            );
                        }
                    }
                }
            }
        }

        if (errors.Any())
        {
            throw new InvalidOperationException(
                $"Validation errors in {Path.GetFileName(filePath)}:\n  - {string.Join("\n  - ", errors)}"
            );
        }
    }
}
