# C# Generator Usage Guide

Complete guide for using the C# FluentValidation code generator.

## Installation

### As a Global Tool

```bash
cd src/csharp-generator
dotnet pack
dotnet tool install --global --add-source ./bin/Debug FluentValidation.CodeGenerator
```

### As a Local Tool

```bash
dotnet new tool-manifest  # If you don't have one already
dotnet tool install --local FluentValidation.CodeGenerator --add-source ./path/to/package
```

### Build from Source

```bash
cd src/csharp-generator
dotnet build
```

## Usage

### Basic Command

```bash
fv-generator generate --input ./rules --output ./Validators
```

### Command Options

```
fv-generator generate [options]

Options:
  -i, --input <path>       Path to directory containing JSON rule files
                           Default: ./rules

  -o, --output <path>      Path to directory for generated C# files
                           Default: ./Validators

  -n, --namespace <name>   Override namespace for generated validators
                           (uses namespace from JSON if not specified)

  --help                   Display help information
  --version                Display version information
```

### Examples

**Generate with custom paths:**

```bash
fv-generator generate \
  --input ./ValidationRules \
  --output ./MyApp/Validators
```

**Override namespace:**

```bash
fv-generator generate \
  --input ./rules \
  --output ./Validators \
  --namespace MyCompany.MyApp.Validators
```

**Using short options:**

```bash
fv-generator generate -i ./rules -o ./Validators -n MyApp.Validators
```

---

## Integration with ASP.NET Core

### Step 1: Generate Validators

```bash
fv-generator generate \
  --input ./rules \
  --output ./Validators \
  --namespace YourApp.Validators
```

### Step 2: Add FluentValidation Package

```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

### Step 3: Register Validators

**Program.cs:**

```csharp
using FluentValidation;
using YourApp.Models;
using YourApp.Validators;

var builder = WebApplication.CreateBuilder(args);

// Register validators
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();

// Or use assembly scanning
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
```

### Step 4: Use in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IValidator<User> _validator;

    public UsersController(IValidator<User> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        var result = await _validator.ValidateAsync(user);

        if (!result.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(
                result.ToDictionary()
            ));
        }

        // Save user...
        return Ok(user);
    }
}
```

---

## Build Pipeline Integration

### Pre-Build Event

Add to your `.csproj` file:

```xml
<Target Name="GenerateValidators" BeforeTargets="BeforeBuild">
  <Exec Command="fv-generator generate --input $(ProjectDir)../rules --output $(ProjectDir)Validators" />
</Target>
```

### MSBuild Task

**Directory.Build.targets:**

```xml
<Project>
  <Target Name="GenerateValidators" BeforeTargets="CoreCompile">
    <PropertyGroup>
      <RulesPath>$(MSBuildThisFileDirectory)rules</RulesPath>
      <OutputPath>$(MSBuildThisFileDirectory)src/Validators</OutputPath>
    </PropertyGroup>

    <Exec Command="fv-generator generate -i $(RulesPath) -o $(OutputPath)" />
  </Target>
</Project>
```

### CI/CD Pipeline

**GitHub Actions:**

```yaml
- name: Generate Validators
  run: |
    dotnet tool install --global FluentValidation.CodeGenerator
    fv-generator generate --input ./rules --output ./src/Validators

- name: Build
  run: dotnet build
```

**Azure DevOps:**

```yaml
- task: DotNetCoreCLI@2
  displayName: "Install Generator"
  inputs:
    command: "custom"
    custom: "tool"
    arguments: "install --global FluentValidation.CodeGenerator"

- script: fv-generator generate -i ./rules -o ./src/Validators
  displayName: "Generate Validators"

- task: DotNetCoreCLI@2
  displayName: "Build Project"
  inputs:
    command: "build"
```

---

## Advanced Usage

### Custom Namespace per Entity

While the generator uses a single namespace per run, you can:

1. Organize rules by namespace in different folders
2. Run the generator multiple times

```bash
# Generate domain validators
fv-generator generate \
  -i ./rules/Domain \
  -o ./Validators/Domain \
  -n MyApp.Domain.Validators

# Generate application validators
fv-generator generate \
  -i ./rules/Application \
  -o ./Validators/Application \
  -n MyApp.Application.Validators
```

### Regenerating Only Changed Files

The generator always overwrites files. Use version control to track changes:

```bash
# Generate validators
fv-generator generate -i ./rules -o ./Validators

# Review changes
git diff Validators/

# Commit if satisfied
git add Validators/
git commit -m "Updated validators"
```

### Validating Multiple Entities

Create a rule file for each entity:

```
rules/
├── User.json
├── Product.json
├── Order.json
└── Payment.json
```

Run once to generate all:

```bash
fv-generator generate -i ./rules -o ./Validators
```

Output:

```
Validators/
├── UserValidator.cs
├── ProductValidator.cs
├── OrderValidator.cs
└── PaymentValidator.cs
```

---

## Output Format

### Generated File Structure

**Input (User.json):**

```json
{
  "entity": "User",
  "namespace": "MyApp.Models",
  "properties": [
    {
      "name": "Email",
      "type": "string",
      "rules": [
        { "validator": "NotEmpty", "message": "Email is required" },
        { "validator": "EmailAddress" }
      ]
    }
  ]
}
```

**Output (UserValidator.cs):**

```csharp
using FluentValidation;

namespace MyApp.Models
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress();
        }
    }
}
```

### File Naming Convention

- Generated files use the pattern: `{EntityName}Validator.cs`
- Examples: `UserValidator.cs`, `ProductValidator.cs`

---

## Troubleshooting

### "Command not found: fv-generator"

**Cause:** Tool not installed globally  
**Solution:**

```bash
dotnet tool install --global FluentValidation.CodeGenerator --add-source ./path/to/package
```

Or use local installation:

```bash
dotnet tool install --local FluentValidation.CodeGenerator
dotnet fv-generator generate ...
```

### "Input directory not found"

**Cause:** Incorrect path to rules directory  
**Solution:** Use absolute path or verify relative path:

```bash
fv-generator generate --input "C:\MyProject\rules" --output ".\Validators"
```

### "Validation errors in {file}.json"

**Cause:** JSON file doesn't match schema  
**Solution:** Check error message for details. Common issues:

- Missing required fields (`entity`, `namespace`, `properties`)
- Empty `rules` array
- Missing `validator` in a rule

### Generated files have compilation errors

**Cause:** Namespace mismatch or missing model classes  
**Solutions:**

1. Ensure model classes exist with correct names
2. Verify namespace matches your project structure
3. Check that property names match exactly (case-sensitive)

### Build fails with "Validator not found"

**Cause:** Validators not registered with dependency injection  
**Solution:** Add to `Program.cs`:

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
```

---

## Tips & Best Practices

### 1. Version Control

- Commit generated validators to source control
- Include JSON rule files in repository
- Review diffs when rules change

### 2. Organize Rules

- Group related entities in subdirectories
- Use consistent naming conventions
- Keep complex rules in separate files

### 3. Automate Generation

- Add to pre-build process
- Include in CI/CD pipeline
- Document generation steps in README

### 4. Testing

- Write unit tests for validators
- Test edge cases
- Verify error messages

### 5. Maintenance

- Regenerate after rule changes
- Keep generator version updated
- Document custom validation requirements

---

## Support

For issues or questions:

- Check [JSON Schema Reference](JSON-SCHEMA-REFERENCE.md)
- Review [sample applications](../samples/README.md)
- Open an issue on GitHub
