# Centralized Validation Rules Engine

A powerful code generation system that eliminates duplicate validation logic across frontend and backend applications by defining validation rules once in JSON and generating both C# FluentValidation and TypeScript fluentvalidation-ts validators automatically.

## 🎯 Overview

Modern applications require validation at multiple layers:

- **Client-side**: During user data entry in forms
- **Server-side**: When data reaches the API/persistence layer

This project solves the problem of **duplicate validation logic** by providing:

1. ✅ **Single Source of Truth**: Define validation rules once in JSON
2. ✅ **C# Code Generator**: Automatically generates FluentValidation classes for .NET
3. ✅ **TypeScript Code Generator**: Automatically generates fluentvalidation-ts classes for frontend
4. ✅ **Perfect Consistency**: Identical validation behavior across frontend and backend

## 🚀 Quick Start

### Define Validation Rules (JSON)

Create a JSON file for your entity (e.g., `rules/User.json`):

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
        { "validator": "EmailAddress", "message": "Invalid email format" }
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
    }
  ]
}
```

### Generate C# Validators

```bash
dotnet fv-generator generate --input ./rules --output ./Validators --namespace MyApp.Validators
```

**Generated Output** (`UserValidator.cs`):

```csharp
using FluentValidation;

namespace MyApp.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 120).WithMessage("Age must be between 18 and 120");
        }
    }
}
```

### Generate TypeScript Validators

```bash
npx fv-ts-generator generate --input ./rules --output ./validators
```

**Generated Output** (`UserValidator.ts`):

```typescript
import { Validator } from "fluentvalidation-ts";

export type User = {
  email: string;
  age: number;
};

export class UserValidator extends Validator<User> {
  constructor() {
    super();

    this.ruleFor("email")
      .notEmpty()
      .withMessage("Email is required")
      .emailAddress()
      .withMessage("Invalid email format");

    this.ruleFor("age")
      .inclusiveBetween(18, 120)
      .withMessage("Age must be between 18 and 120");
  }
}
```

## 📁 Project Structure

```
fluentValidation-cli-claude/
├── schema/                     # JSON schema definition
├── rules/                      # Entity validation definitions
│   ├── User.json
│   ├── Product.json
│   └── Address.json
├── src/
│   ├── csharp-generator/      # C# code generator CLI
│   └── typescript-generator/   # TypeScript code generator CLI
├── samples/
│   ├── backend/               # ASP.NET Core sample app
│   └── frontend/              # React + TypeScript sample app
└── docs/                      # Comprehensive documentation
```

## 📚 Documentation

- **[JSON Schema Reference](docs/JSON-SCHEMA-REFERENCE.md)** - Complete guide to validation rule syntax
- **[C# Generator Guide](docs/CSHARP-GENERATOR-GUIDE.md)** - Using the .NET code generator
- **[TypeScript Generator Guide](docs/TYPESCRIPT-GENERATOR-GUIDE.md)** - Using the Node.js code generator
- **[Integration Guide](docs/INTEGRATION-GUIDE.md)** - Integrating validators into your applications
- **[Architecture](docs/ARCHITECTURE.md)** - System design and extension points

## ✨ Features

### Supported Validators

The system supports 20+ validation rules that work identically in both C# and TypeScript:

| Validator          | Description                             | Example                                   |
| ------------------ | --------------------------------------- | ----------------------------------------- |
| `NotNull`          | Value must not be null                  | `"validator": "NotNull"`                  |
| `NotEmpty`         | Value must not be empty/null/whitespace | `"validator": "NotEmpty"`                 |
| `EmailAddress`     | Must be valid email format              | `"validator": "EmailAddress"`             |
| `Length`           | String length must be in range          | `"parameters": { "min": 2, "max": 50 }`   |
| `MinLength`        | Minimum string length                   | `"parameters": { "length": 5 }`           |
| `MaxLength`        | Maximum string length                   | `"parameters": { "length": 100 }`         |
| `Matches`          | Must match regex pattern                | `"parameters": { "pattern": "^[A-Z]+$" }` |
| `GreaterThan`      | Numeric value > threshold               | `"parameters": { "value": 0 }`            |
| `LessThan`         | Numeric value < threshold               | `"parameters": { "value": 100 }`          |
| `InclusiveBetween` | Value in range (inclusive)              | `"parameters": { "min": 1, "max": 10 }`   |
| `ExclusiveBetween` | Value in range (exclusive)              | `"parameters": { "min": 0, "max": 100 }`  |

[See full validator list →](docs/JSON-SCHEMA-REFERENCE.md)

### Custom Error Messages

Every validation rule supports custom error messages:

```json
{
  "validator": "NotEmpty",
  "message": "Please enter your email address"
}
```

## 🛠️ Installation

### C# Generator

```bash
dotnet tool install --global FluentValidation.Generator
```

### TypeScript Generator

```bash
npm install -g @fv-cli/typescript-generator
```

## 🎨 Sample Applications

The `samples/` folder contains fully functional examples:

- **Backend (ASP.NET Core)**: Web API with generated validators integrated
- **Frontend (React + TypeScript)**: Form validation using generated validators

Run the samples:

```bash
# Backend
cd samples/backend
dotnet run

# Frontend
cd samples/frontend
npm install && npm start
```

## 🧪 Benefits

- ✅ **Save Development Time**: Write validation rules once, use everywhere
- ✅ **Eliminate Bugs**: No more frontend/backend validation mismatches
- ✅ **Easy Maintenance**: Update rules in one place, regenerate code
- ✅ **Type Safety**: Fully typed validators for both C# and TypeScript
- ✅ **Professional UX**: Consistent error messages across all platforms

## 🤝 Contributing

Contributions are welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 📄 License

This project is licensed under the MIT License - see [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📖 [Documentation](docs/)
- 🐛 [Issue Tracker](https://github.com/yourusername/fluentvalidation-cli/issues)
- 💬 [Discussions](https://github.com/yourusername/fluentvalidation-cli/discussions)

---

**Built with** ❤️ **by .NET experts for modern full-stack development**
