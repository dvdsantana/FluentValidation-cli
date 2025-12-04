namespace FluentValidation.Generator.Services;

/// <summary>
/// Handles writing generated code to the file system
/// </summary>
public class FileWriter
{
    /// <summary>
    /// Writes generated validator code to a file
    /// </summary>
    /// <param name="outputPath">Directory where the file should be written</param>
    /// <param name="fileName">Name of the file (e.g., "UserValidator.cs")</param>
    /// <param name="content">Generated code content</param>
    public async Task WriteFileAsync(string outputPath, string fileName, string content)
    {
        // Create output directory if it doesn't exist
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
            Console.WriteLine($"Created output directory: {outputPath}");
        }

        var fullPath = Path.Combine(outputPath, fileName);

        // Write the file
        await File.WriteAllTextAsync(fullPath, content);

        Console.WriteLine($"✓ Generated {fileName}");
    }

    /// <summary>
    /// Writes multiple validator files
    /// </summary>
    /// <param name="outputPath">Directory where files should be written</param>
    /// <param name="files">Dictionary of filename -> content pairs</param>
    public async Task WriteFilesAsync(string outputPath, Dictionary<string, string> files)
    {
        foreach (var (fileName, content) in files)
        {
            await WriteFileAsync(outputPath, fileName, content);
        }
    }
}
