using System.Text.Json;
using FluentValidationCLI.Generator.Generators;
using FluentValidationCLI.Generator.Models;

namespace FluentValidationCLI.Generator;

class Program
{
    static void Main(string[] args)
    {
        string inputDir = "rules";
        string outputCSharp = "src/FluentValidationCLI.Backend/Validators";
        string outputTS = "src/FluentValidationCLI.Frontend/src/validators";
        string namespaceName = "FluentValidationCLI.Backend.Validators";

        // Simple arg parsing
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--input" && i + 1 < args.Length)
                inputDir = args[i + 1];
            if (args[i] == "--output-csharp" && i + 1 < args.Length)
                outputCSharp = args[i + 1];
            if (args[i] == "--output-ts" && i + 1 < args.Length)
                outputTS = args[i + 1];
            if (args[i] == "--namespace" && i + 1 < args.Length)
                namespaceName = args[i + 1];
        }

        if (!Directory.Exists(inputDir))
        {
            Console.WriteLine($"Input directory '{inputDir}' does not exist.");
            return;
        }

        Directory.CreateDirectory(outputCSharp);
        Directory.CreateDirectory(outputTS);

        var csharpGenerator = new CSharpGenerator();
        var tsGenerator = new TypeScriptGenerator();

        foreach (var file in Directory.GetFiles(inputDir, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(file);
                var ruleDef = JsonSerializer.Deserialize<RuleDefinition>(json);

                if (ruleDef == null)
                {
                    Console.WriteLine($"Failed to deserialize {file}");
                    continue;
                }

                // Generate C#
                string csharpCode = csharpGenerator.Generate(ruleDef, namespaceName);
                File.WriteAllText(
                    Path.Combine(outputCSharp, $"{ruleDef.EntityName}Validator.cs"),
                    csharpCode
                );
                Console.WriteLine($"Generated C# validator for {ruleDef.EntityName}");

                // Generate TS
                string tsCode = tsGenerator.Generate(ruleDef);
                File.WriteAllText(
                    Path.Combine(outputTS, $"{ruleDef.EntityName}Validator.ts"),
                    tsCode
                );
                Console.WriteLine($"Generated TS validator for {ruleDef.EntityName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {file}: {ex.Message}");
            }
        }
    }
}
