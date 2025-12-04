using System.CommandLine;
using FluentValidation.Generator.Services;

namespace FluentValidation.Generator;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(
            "FluentValidation Code Generator - Generate C# FluentValidation classes from JSON rule definitions"
        );

        var generateCommand = new Command(
            "generate",
            "Generate C# validator classes from JSON rule definitions"
        );

        var inputOption = new Option<string>(
            name: "--input",
            description: "Path to directory containing JSON rule definition files",
            getDefaultValue: () => "./rules"
        );
        inputOption.AddAlias("-i");

        var outputOption = new Option<string>(
            name: "--output",
            description: "Path to directory where generated C# files should be written",
            getDefaultValue: () => "./Validators"
        );
        outputOption.AddAlias("-o");

        var namespaceOption = new Option<string?>(
            name: "--namespace",
            description: "Override namespace for generated validators (uses namespace from JSON if not specified)"
        );
        namespaceOption.AddAlias("-n");

        generateCommand.AddOption(inputOption);
        generateCommand.AddOption(outputOption);
        generateCommand.AddOption(namespaceOption);

        generateCommand.SetHandler(
            async (string input, string output, string? namespaceOverride) =>
            {
                await GenerateValidatorsAsync(input, output, namespaceOverride);
            },
            inputOption,
            outputOption,
            namespaceOption
        );

        rootCommand.AddCommand(generateCommand);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task GenerateValidatorsAsync(
        string inputPath,
        string outputPath,
        string? namespaceOverride
    )
    {
        try
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║       FluentValidation C# Code Generator v1.0.0               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine($"Input:     {Path.GetFullPath(inputPath)}");
            Console.WriteLine($"Output:    {Path.GetFullPath(outputPath)}");
            if (!string.IsNullOrWhiteSpace(namespaceOverride))
            {
                Console.WriteLine($"Namespace: {namespaceOverride} (override)");
            }
            Console.WriteLine();

            // Parse JSON files
            Console.WriteLine("Parsing JSON rule definitions...");
            var parser = new JsonParser();
            var definitions = await parser.ParseDirectoryAsync(inputPath);

            if (definitions.Count == 0)
            {
                Console.WriteLine("No validation definitions found. Exiting.");
                return;
            }

            Console.WriteLine($"Found {definitions.Count} entity definition(s)");
            Console.WriteLine();

            // Apply namespace override if provided
            if (!string.IsNullOrWhiteSpace(namespaceOverride))
            {
                foreach (var def in definitions)
                {
                    def.Namespace = namespaceOverride;
                }
            }

            // Generate code
            Console.WriteLine("Generating C# validators...");
            var codeGenerator = new CSharpCodeGenerator();
            var fileWriter = new FileWriter();

            foreach (var definition in definitions)
            {
                var code = codeGenerator.GenerateValidator(definition);
                var fileName = codeGenerator.GetValidatorFileName(definition);
                await fileWriter.WriteFileAsync(outputPath, fileName, code);
            }

            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  ✓ Successfully generated {definitions.Count} validator(s)");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ✗ Error occurred during code generation");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine();

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }

            Environment.Exit(1);
        }
    }
}
