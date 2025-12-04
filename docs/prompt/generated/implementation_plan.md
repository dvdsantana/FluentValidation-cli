# Centralized Validation Rules Engine - Implementation Plan

## Executive Summary

This implementation plan details the development of a **Centralized Validation Rules Engine** that serves as a single source of truth for validation logic across frontend (TypeScript) and backend (C#) applications. The system will:

1. Define validation rules in technology-agnostic JSON files
2. Generate C# FluentValidation classes for backend validation
3. Generate TypeScript fluentvalidation-ts classes for frontend validation
4. Ensure consistency and eliminate duplicate validation code

---

## User Review Required

> [!IMPORTANT] > **Technology Stack Decisions**
>
> - **Backend**: C# .NET 8+ with FluentValidation 11.x library
> - **Frontend**: TypeScript with fluentvalidation-ts library
> - **JSON Schema**: Custom JSON schema for validation rules (not JSON Schema standard)
> - **CLI Tools**: .NET CLI tool for C# generation, Node.js CLI for TypeScript generation
>
> Please confirm these technology choices align with your requirements.

> [!IMPORTANT] > **Code Generation Approach**
> The system will generate complete validator classes from scratch, overwriting existing files. If you need to preserve custom business logic validators (e.g., database uniqueness checks), those should be kept in separate validator classes and composed using FluentValidation's `Include` mechanism.

> [!WARNING] > **Async Validation Limitation**
> The JSON-based approach will focus on synchronous, structural validation rules only. Complex business rules requiring async operations (database checks, external API calls) must be implemented manually in separate validator classes.

---

## Proposed Changes

### JSON Schema & Sample Definitions

#### [NEW] [schema.json](file:///f:/Projects/fluentValidation-cli-claude/schema/validation-schema.json)

Defines the JSON schema structure for validation rules. This is the contract that all entity validation files must follow.

**Schema Structure:**

```json
{
  "entity": "EntityName",
  "namespace": "YourApp.Models",
  "properties": [
    {
      "name": "PropertyName",
      "type": "string|number|boolean|date",
      "rules": [
        {
          "validator": "NotEmpty|NotNull|Length|Email|GreaterThan|etc",
          "parameters": {},
          "message": "Custom error message",
          "when": "conditional expression (optional)"
        }
      ]
    }
  ]
}
```

**Key Features:**

- Entity-level metadata (name, namespace for C#, module path for TypeScript)
- Property-level rule definitions
- Support for 20+ common validators (matching FluentValidation and fluentvalidation-ts capabilities)
- Custom error messages per rule
- Conditional validation support

---

#### [NEW] [User.json](file:///f:/Projects/fluentValidation-cli-claude/rules/User.json)

Sample validation rules for a User entity demonstrating common patterns:

```json
{
  "entity": "User",
  "namespace": "SampleApp.Models",
  "properties": [
    {
      "name": "Id",
      "type": "number",
      "rules": [
        {
          "validator": "NotNull",
          "message": "User ID is required"
        }
      ]
    },
    {
      "name": "Email",
      "type": "string",
      "rules": [
        {
          "validator": "NotEmpty",
          "message": "Email is required"
        },
        {
          "validator": "EmailAddress",
          "message": "Please provide a valid email address"
        },
        {
          "validator": "MaxLength",
          "parameters": { "length": 255 },
          "message": "Email must not exceed 255 characters"
        }
      ]
    },
    {
      "name": "Age",
      "type": "number",
      "rules": [
        {
          "validator": "InclusiveBetween",
          "parameters": { "min": 18, "max": 120 },
          "message": "Age must be between 18 and 120"
        }
      ]
    },
    {
      "name": "Name",
      "type": "string",
      "rules": [
        {
          "validator": "NotEmpty",
          "message": "Name is required"
        },
        {
          "validator": "Length",
          "parameters": { "min": 2, "max": 100 },
          "message": "Name must be between 2 and 100 characters"
        }
      ]
    }
  ]
}
```

---

#### [NEW] [Product.json](file:///f:/Projects/fluentValidation-cli-claude/rules/Product.json)

Sample validation rules for a Product entity:

```json
{
  "entity": "Product",
  "namespace": "SampleApp.Models",
  "properties": [
    {
      "name": "Name",
      "type": "string",
      "rules": [
        {
          "validator": "NotEmpty",
          "message": "Product name is required"
        },
        {
          "validator": "MaxLength",
          "parameters": { "length": 200 },
          "message": "Product name cannot exceed 200 characters"
        }
      ]
    },
    {
      "name": "Price",
      "type": "number",
      "rules": [
        {
          "validator": "GreaterThan",
          "parameters": { "value": 0 },
          "message": "Price must be greater than 0"
        },
        {
          "validator": "LessThanOrEqualTo",
          "parameters": { "value": 100000 },
          "message": "Price cannot exceed 100,000"
        }
      ]
    },
    {
      "name": "Sku",
      "type": "string",
      "rules": [
        {
          "validator": "NotEmpty",
          "message": "SKU is required"
        },
        {
          "validator": "Matches",
          "parameters": { "pattern": "^[A-Z]{3}-\\d{4}$" },
          "message": "SKU must follow format: XXX-9999"
        }
      ]
    }
  ]
}
```

---

### C# Code Generator

#### [NEW] [FluentValidation.Generator.csproj](file:///f:/Projects/fluentValidation-cli-claude/src/csharp-generator/FluentValidation.Generator.csproj)

.NET CLI tool project that generates C# FluentValidation classes.

**Dependencies:**

- .NET 8.0 SDK
- FluentValidation (11.x)
- System.Text.Json
- System.CommandLine (for CLI interface)

**PackageType:** DotnetTool for global/local installation

---

#### [NEW] [Program.cs](file:///f:/Projects/fluentValidation-cli-claude/src/csharp-generator/Program.cs)

Main entry point for the C# generator CLI tool.

**Features:**

- Command-line argument parsing (input path, output path, namespace)
- Orchestrates the generation process
- Error handling and logging
- Help documentation

**CLI Usage:**

```bash
dotnet fv-generator generate --input ./rules --output ./Validators --namespace MyApp.Validators
```

---

#### [NEW] [JsonParser.cs](file:///f:/Projects/fluentValidation-cli-claude/src/csharp-generator/Services/JsonParser.cs)

Parses and validates JSON rule files.

**Responsibilities:**

- Read JSON files from input directory
- Deserialize to strongly-typed C# models
- Validate against schema
- Report parsing errors with clear messages

---

#### [NEW] [ValidationRuleModels.cs](file:///f:/Projects/fluentValidation-cli-claude/src/csharp-generator/Models/ValidationRuleModels.cs)

C# models representing the JSON schema structure.

**Key Classes:**

- `ValidationDefinition`: Root entity definition
- `PropertyValidation`: Property-level validation
- `ValidationRule`: Individual rule with validator type and parameters

---

#### [NEW] [CSharpCodeGenerator.cs](file:///f:/Projects/fluentValidation-cli-claude/src/csharp-generator/Services/CSharpCodeGenerator.cs)

Core code generation engine for C# FluentValidation classes.

**Responsibilities:**

- Generate `AbstractValidator<TEntity>` class structure
- Map JSON validators to FluentValidation methods
- Generate `RuleFor` chains
- Handle custom messages with `WithMessage`
- Handle conditional rules with `When`/`Unless`
- Format code with proper indentation

**Example Output:**

```csharp
using FluentValidation;

namespace SampleApp.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("User ID is required");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Please provide a valid email address")
                .MaximumLength(255)
                .WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 120)
                .WithMessage("Age must be between 18 and 120");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .Length(2, 100)
                .WithMessage("Name must be between 2 and 100 characters");
        }
    }
}
```

---

#### [NEW] [ValidatorMapping.cs](file:///f:/Projects/fluentValidation-cli-claude/src/csharp-generator/Mapping/ValidatorMapping.cs)

Maps JSON validator names to C# FluentValidation method calls.

**Supported Validators:**

- NotNull → `NotNull()`
- NotEmpty → `NotEmpty()`
- Empty → `Empty()`
- Length → `Length(min, max)`
- MinLength → `MinimumLength(length)`
- MaxLength → `MaximumLength(length)`
- EmailAddress → `EmailAddress()`
- Matches → `Matches(pattern)`
- Equal → `Equal(value)`
- NotEqual → `NotEqual(value)`
- LessThan → `LessThan(value)`
- LessThanOrEqualTo → `LessThanOrEqualTo(value)`
- GreaterThan → `GreaterThan(value)`
- GreaterThanOrEqualTo → `GreaterThanOrEqualTo(value)`
- InclusiveBetween → `InclusiveBetween(min, max)`
- ExclusiveBetween → `ExclusiveBetween(min, max)`
- CreditCard → `CreditCard()`
- IsInEnum → `IsInEnum()`

---

#### [NEW] [FileWriter.cs](file:///f:/Projects/fluentValidation-cli-claude/src/csharp-generator/Services/FileWriter.cs)

Handles writing generated code to files.

**Features:**

- Create output directory if needed
- Write files with UTF-8 encoding
- Handle file naming conventions (EntityNameValidator.cs)
- Overwrite warning/confirmation

---

### TypeScript Code Generator

#### [NEW] [package.json](file:///f:/Projects/fluentValidation-cli-claude/src/typescript-generator/package.json)

Node.js CLI tool project that generates TypeScript fluentvalidation-ts classes.

**Dependencies:**

- fluentvalidation-ts
- commander (CLI framework)
- TypeScript
- ts-node

**bin entry:** Allows global npm installation

---

#### [NEW] [cli.ts](file:///f:/Projects/fluentValidation-cli-claude/src/typescript-generator/src/cli.ts)

Main entry point for the TypeScript generator CLI tool.

**Features:**

- Command-line argument parsing using commander
- Orchestrates generation process
- Error handling and logging

**CLI Usage:**

```bash
npx fv-ts-generator generate --input ./rules --output ./validators
```

---

#### [NEW] [jsonParser.ts](file:///f:/Projects/fluentValidation-cli-claude/src/typescript-generator/src/services/jsonParser.ts)

Parses and validates JSON rule files (TypeScript version).

**Responsibilities:**

- Read JSON files from input directory
- Parse to TypeScript interfaces
- Validate structure
- Report errors

---

#### [NEW] [validationRuleModels.ts](file:///f:/Projects/fluentValidation-cli-claude/src/typescript-generator/src/models/validationRuleModels.ts)

TypeScript interfaces representing the JSON schema.

**Key Interfaces:**

- `ValidationDefinition`: Root entity definition
- `PropertyValidation`: Property-level validation
- `ValidationRule`: Individual rule definition

---

#### [NEW] [typeScriptCodeGenerator.ts](file:///f:/Projects/fluentValidation-cli-claude/src/typescript-generator/src/services/typeScriptCodeGenerator.ts)

Core code generation engine for TypeScript fluentvalidation-ts classes.

**Responsibilities:**

- Generate `Validator<TModel>` class structure
- Map JSON validators to fluentvalidation-ts methods
- Generate `ruleFor` chains
- Handle custom messages with `withMessage`
- Format code with proper indentation

**Example Output:**

```typescript
import { Validator } from "fluentvalidation-ts";

export type User = {
  id: number;
  email: string;
  age: number;
  name: string;
};

export class UserValidator extends Validator<User> {
  constructor() {
    super();

    this.ruleFor("id").notNull().withMessage("User ID is required");

    this.ruleFor("email")
      .notEmpty()
      .withMessage("Email is required")
      .emailAddress()
      .withMessage("Please provide a valid email address")
      .maxLength(255)
      .withMessage("Email must not exceed 255 characters");

    this.ruleFor("age")
      .inclusiveBetween(18, 120)
      .withMessage("Age must be between 18 and 120");

    this.ruleFor("name")
      .notEmpty()
      .withMessage("Name is required")
      .length(2, 100)
      .withMessage("Name must be between 2 and 100 characters");
  }
}
```

---

#### [NEW] [validatorMapping.ts](file:///f:/Projects/fluentValidation-cli-claude/src/typescript-generator/src/mapping/validatorMapping.ts)

Maps JSON validator names to TypeScript fluentvalidation-ts method calls.

**Supported Validators:**

- NotEmpty → `notEmpty()`
- EmailAddress → `emailAddress()`
- Equal → `equal(value)`
- ExclusiveBetween → `exclusiveBetween(min, max)`
- GreaterThan → `greaterThan(value)`
- GreaterThanOrEqualTo → `greaterThanOrEqualTo(value)`
- InclusiveBetween → `inclusiveBetween(min, max)`
- Length → `length(min, max)`
- LessThan → `lessThan(value)`
- LessThanOrEqualTo → `lessThanOrEqualTo(value)`
- Matches → `matches(pattern)`
- MaxLength → `maxLength(length)`
- MinLength → `minLength(length)`
- Must → `must(predicate)` (for custom rules)

---

### Sample Backend Application (C# / ASP.NET Core)

#### [NEW] [SampleApi.csproj](file:///f:/Projects/fluentValidation-cli-claude/samples/backend/SampleApi.csproj)

ASP.NET Core Web API demonstrating generated validator usage.

**Dependencies:**

- ASP.NET Core 8.0
- FluentValidation
- FluentValidation.DependencyInjectionExtensions

---

#### [NEW] [User.cs](file:///f:/Projects/fluentValidation-cli-claude/samples/backend/Models/User.cs)

User model matching the JSON validation definition.

```csharp
namespace SampleApp.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
    }
}
```

---

#### [NEW] [UserController.cs](file:///f:/Projects/fluentValidation-cli-claude/samples/backend/Controllers/UserController.cs)

API controller demonstrating manual validation approach.

```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IValidator<User> _validator;

    public UserController(IValidator<User> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var result = await _validator.ValidateAsync(user);

        if (!result.IsValid)
        {
            return BadRequest(result.ToDictionary());
        }

        // Save user logic here
        return Ok(user);
    }
}
```

---

#### [MODIFY] [Program.cs](file:///f:/Projects/fluentValidation-cli-claude/samples/backend/Program.cs)

Configure validator dependency injection.

```csharp
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();
```

---

### Sample Frontend Application (TypeScript / React)

#### [NEW] [package.json](file:///f:/Projects/fluentValidation-cli-claude/samples/frontend/package.json)

React + TypeScript application demonstrating generated validator usage.

**Dependencies:**

- React 18
- TypeScript
- fluentvalidation-ts
- formik (optional, for form handling)

---

#### [NEW] [UserForm.tsx](file:///f:/Projects/fluentValidation-cli-claude/samples/frontend/src/components/UserForm.tsx)

React component with client-side validation using generated validator.

```typescript
import { useState } from "react";
import { UserValidator } from "../validators/UserValidator";

const formValidator = new UserValidator();

export function UserForm() {
  const [formData, setFormData] = useState({
    id: null,
    email: "",
    age: 0,
    name: "",
  });

  const [errors, setErrors] = useState({});

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const validationErrors = formValidator.validate(formData);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    // Submit to API
    await fetch("/api/user", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(formData),
    });
  };

  return (
    <form onSubmit={handleSubmit}>{/* Form fields with error display */}</form>
  );
}
```

---

### Documentation

#### [NEW] [README.md](file:///f:/Projects/fluentValidation-cli-claude/README.md)

Main project README with overview, quick start, and links to detailed docs.

---

#### [NEW] [JSON-SCHEMA-REFERENCE.md](file:///f:/Projects/fluentValidation-cli-claude/docs/JSON-SCHEMA-REFERENCE.md)

Complete reference for the JSON validation schema format.

**Contents:**

- Schema structure
- Supported validators
- Parameter reference
- Examples for each validator
- Conditional validation examples

---

#### [NEW] [CSHARP-GENERATOR-GUIDE.md](file:///f:/Projects/fluentValidation-cli-claude/docs/CSHARP-GENERATOR-GUIDE.md)

Guide for using the C# code generator.

**Contents:**

- Installation instructions
- CLI usage
- Integration with build pipelines
- Troubleshooting

---

#### [NEW] [TYPESCRIPT-GENERATOR-GUIDE.md](file:///f:/Projects/fluentValidation-cli-claude/docs/TYPESCRIPT-GENERATOR-GUIDE.md)

Guide for using the TypeScript code generator.

**Contents:**

- Installation instructions
- CLI usage
- Integration with npm scripts
- Troubleshooting

---

#### [NEW] [INTEGRATION-GUIDE.md](file:///f:/Projects/fluentValidation-cli-claude/docs/INTEGRATION-GUIDE.md)

Guide for integrating validators into applications.

**Contents:**

- ASP.NET Core integration (manual & automatic)
- React integration
- Minimal API integration
- Custom business rules composition
- Error message internationalization

---

## Verification Plan

### Automated Tests

#### C# Generator Unit Tests

```bash
cd src/csharp-generator
dotnet test
```

**Test Coverage:**

- JSON parsing with valid/invalid schemas
- Code generation for all validator types
- Custom message handling
- Conditional validation rules
- File output

#### TypeScript Generator Unit Tests

```bash
cd src/typescript-generator
npm test
```

**Test Coverage:**

- JSON parsing with valid/invalid schemas
- Code generation for all validator types
- Custom message handling
- File output

#### End-to-End Integration Tests

```bash
# Generate validators from sample JSON files
dotnet fv-generator generate --input ./rules --output ./samples/backend/Validators
npx fv-ts-generator generate --input ./rules --output ./samples/frontend/src/validators

# Run backend tests
cd samples/backend
dotnet test

# Run frontend tests
cd samples/frontend
npm test
```

**Test Coverage:**

- Generated C# validators work with ASP.NET Core
- Generated TypeScript validators work in browser
- Both validators produce identical validation results
- Error messages match between frontend/backend

### Manual Verification

#### Scenario 1: User Registration Form

1. Start the sample backend API:
   ```bash
   cd samples/backend
   dotnet run
   ```
2. Start the sample frontend:
   ```bash
   cd samples/frontend
   npm start
   ```
3. Open browser to `http://localhost:3000`
4. Test User registration form:
   - Submit empty form → Should show all required field errors
   - Enter invalid email → Should show email format error
   - Enter age < 18 → Should show age range error
   - Enter valid data → Should submit successfully
5. Verify backend API returns same validation errors when called directly via Postman/curl

#### Scenario 2: Code Generation

1. Modify `rules/User.json` to add a new validation rule (e.g., phone number)
2. Run C# generator:
   ```bash
   dotnet fv-generator generate --input ./rules --output ./samples/backend/Validators
   ```
3. Run TypeScript generator:
   ```bash
   npx fv-ts-generator generate --input ./rules --output ./samples/frontend/src/validators
   ```
4. Verify both generated files contain the new rule
5. Rebuild and test both frontend/backend applications
6. Confirm new validation works on both sides

#### Scenario 3: Validator Composition

1. Create a custom `UserBusinessRulesValidator` that checks username uniqueness (async database call)
2. Compose it with the generated `UserValidator` using FluentValidation's `Include`
3. Test that both structural validation and business rules work together
4. Verify error messages from both validators are merged correctly

---

## Implementation Notes

### Recommended Project Structure

```
fluentValidation-cli-claude/
├── schema/
│   └── validation-schema.json         # JSON schema definition
├── rules/
│   ├── User.json                      # Sample entity rules
│   ├── Product.json
│   └── Address.json
├── src/
│   ├── csharp-generator/              # C# code generator
│   │   ├── FluentValidation.Generator.csproj
│   │   ├── Program.cs
│   │   ├── Models/
│   │   │   └── ValidationRuleModels.cs
│   │   ├── Services/
│   │   │   ├── JsonParser.cs
│   │   │   ├── CSharpCodeGenerator.cs
│   │   │   └── FileWriter.cs
│   │   └── Mapping/
│   │       └── ValidatorMapping.cs
│   └── typescript-generator/          # TypeScript code generator
│       ├── package.json
│       ├── tsconfig.json
│       └── src/
│           ├── cli.ts
│           ├── models/
│           │   └── validationRuleModels.ts
│           ├── services/
│           │   ├── jsonParser.ts
│           │   ├── typeScriptCodeGenerator.ts
│           │   └── fileWriter.ts
│           └── mapping/
│               └── validatorMapping.ts
├── samples/
│   ├── backend/                       # ASP.NET Core sample
│   │   ├── SampleApi.csproj
│   │   ├── Program.cs
│   │   ├── Models/
│   │   │   ├── User.cs
│   │   │   └── Product.cs
│   │   ├── Controllers/
│   │   │   └── UserController.cs
│   │   └── Validators/               # Generated validators go here
│   │       ├── UserValidator.cs
│   │       └── ProductValidator.cs
│   └── frontend/                      # React + TypeScript sample
│       ├── package.json
│       ├── src/
│       │   ├── components/
│       │   │   └── UserForm.tsx
│       │   ├── validators/           # Generated validators go here
│       │   │   ├── UserValidator.ts
│       │   │   └── ProductValidator.ts
│       │   └── App.tsx
│       └── public/
├── docs/
│   ├── JSON-SCHEMA-REFERENCE.md
│   ├── CSHARP-GENERATOR-GUIDE.md
│   ├── TYPESCRIPT-GENERATOR-GUIDE.md
│   ├── INTEGRATION-GUIDE.md
│   └── ARCHITECTURE.md
└── README.md
```

### Key Design Decisions

1. **Separate CLI Tools**: C# and TypeScript generators are separate tools to avoid cross-platform complexity
2. **Overwrite Strategy**: Generators always overwrite output files to ensure consistency
3. **Custom Business Rules**: Keep manual validators separate and use composition
4. **Type Safety**: Generate TypeScript type definitions alongside validators
5. **Error Messages**: Support custom messages in JSON, with sensible defaults

### Extension Points

- Custom validator mappings for domain-specific rules
- Template-based code generation for customization
- Multi-language support for error messages
- Integration with OpenAPI/Swagger for API model validation
