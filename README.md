# FluentValidationCLI - Centralized Validation Rules Engine

A powerful code generation tool that eliminates redundant validation logic by generating both C# (FluentValidation) and TypeScript (fluentvalidation-ts) validators from a single JSON source of truth.

## 🎯 Problem Statement

Modern applications require data validation at multiple stages:

- **Client-Side**: During user data entry (frontend)
- **Server-Side**: Upon data reception by the persistence layer (backend)

Traditionally, this means writing the same validation rules twice—once in JavaScript/TypeScript for the frontend and once in C# for the backend. This leads to:

- ❌ Increased development and maintenance effort
- ❌ Risk of inconsistency between client and server validation
- ❌ Duplicate code that must be kept in sync manually

## ✨ Solution

FluentValidationCLI provides a **technology-agnostic, centralized system** for defining validation rules. Write your validation rules once in JSON, and automatically generate validators for both your backend and frontend.

### Key Benefits

- ✅ **Efficiency**: Drastically reduce development time by eliminating duplicate coding
- ✅ **Consistency**: Guarantee that frontend and backend validations are always aligned
- ✅ **Maintainability**: Changes to validation logic are made in one place and propagated automatically
- ✅ **Clarity**: Transparent, declarative documentation of all validation rules

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later
- [Node.js](https://nodejs.org/) 18.x or later
- [npm](https://www.npmjs.com/) 10.x or later

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/yourusername/FluentValidationCLI.git
   cd FluentValidationCLI
   ```

2. **Restore .NET dependencies:**

   ```bash
   dotnet restore
   ```

3. **Install frontend dependencies:**
   ```bash
   cd src/FluentValidationCLI.Frontend
   npm install
   cd ../..
   ```

## 📖 Usage

### 1. Define Your Validation Rules

Create or edit JSON files in the `rules/` directory. Each JSON file represents validation rules for a specific entity.

**Example: `rules/User.json`**

```json
{
  "entityName": "User",
  "properties": [
    {
      "name": "Username",
      "type": "string",
      "rules": [
        { "type": "NotEmpty", "message": "Username is required" },
        {
          "type": "Length",
          "min": 3,
          "max": 20,
          "message": "Username must be between 3 and 20 chars"
        }
      ]
    },
    {
      "name": "Email",
      "type": "string",
      "rules": [{ "type": "NotEmpty" }, { "type": "EmailAddress" }]
    },
    {
      "name": "Age",
      "type": "number",
      "rules": [
        { "type": "GreaterThan", "value": 18 },
        { "type": "LessThan", "value": 100 }
      ]
    }
  ]
}
```

### 2. Generate Validators

Run the generator to create validation code for both backend and frontend:

```bash
dotnet run --project src/FluentValidationCLI.Generator/FluentValidationCLI.Generator.csproj \
```

**Output:**

- C# validators in `src/FluentValidationCLI.Backend/Validators/`
- TypeScript validators in `src/FluentValidationCLI.Frontend/src/validators/`

**Optional CLI Arguments:**

```bash
dotnet run --project src/FluentValidationCLI.Generator/FluentValidationCLI.Generator.csproj \ \
  --input rules \
  --output-csharp src/FluentValidationCLI.Backend/Validators \
  --output-ts src/FluentValidationCLI.Frontend/src/validators \
  --namespace FluentValidationCLI.Backend.Validators
```

### 3. Use Generated Validators

#### Backend (ASP.NET Core)

The generated validators integrate seamlessly with FluentValidation:

```csharp
using FluentValidation;
using FluentValidationCLI.Backend.Models;
using FluentValidationCLI.Backend.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

var app = builder.Build();

app.MapPost("/validate-user", async (IValidator<User> validator, User user) =>
{
    var result = await validator.ValidateAsync(user);
    if (!result.IsValid)
    {
        return Results.ValidationProblem(result.ToDictionary());
    }
    return Results.Ok("User is valid!");
});

app.Run();
```

#### Frontend (React + TypeScript)

Use the generated TypeScript validators in your React components:

```typescript
import { useState } from "react";
import { UserValidator } from "./validators/UserValidator";
import type { User } from "./validators/UserValidator";

function App() {
  const [user, setUser] = useState<User>({
    username: "",
    email: "",
    age: 0,
  });
  const [errors, setErrors] = useState<any>({});

  const validator = new UserValidator();

  const validate = () => {
    const result = validator.validate(user);
    setErrors(result);
  };

  return (
    <div>
      <input name="username" value={user.username} onChange={handleChange} />
      {errors.username && <span>{errors.username}</span>}
      <button onClick={validate}>Validate</button>
    </div>
  );
}
```

## 📁 Project Structure

```
FluentValidationCLI/
├── rules/                          # JSON validation rule definitions
│   └── User.json
├── src/
│   ├── FluentValidationCLI.Generator/       # Code generator console app
│   │   ├── Models/                # JSON deserialization models
│   │   ├── Generators/            # C# and TypeScript generators
│   │   └── Program.cs             # CLI entry point
│   ├── FluentValidationCLI.Backend/         # Sample ASP.NET Core API
│   │   ├── Models/                # Entity models
│   │   ├── Validators/            # Generated C# validators
│   │   └── Program.cs
│   └── FluentValidationCLI.Frontend/        # Sample React TypeScript app
│       └── src/
│           └── validators/        # Generated TypeScript validators
├── docs/                          # Documentation
└── README.md
```

## 🛠️ Supported Validation Rules

### Current Support

| Rule Type    | C# (FluentValidation) | TypeScript (fluentvalidation-ts) |
| ------------ | --------------------- | -------------------------------- |
| NotEmpty     | ✅                    | ✅                               |
| Length       | ✅                    | ✅                               |
| EmailAddress | ✅                    | ✅                               |
| GreaterThan  | ✅                    | ✅                               |
| LessThan     | ✅                    | ✅                               |

### Coming Soon

- Regex/Matches
- Custom validators
- Conditional validation (When)
- Cross-property validation
- Async validation rules

## 🧪 Running the Examples

### Backend API

```bash
dotnet run --project src/FluentValidationCLI.Backend/FluentValidationCLI.Backend.csproj
```

Visit `https://localhost:5001/swagger` to test the API.

### Frontend App

```bash
cd src/FluentValidationCLI.Frontend
npm run dev
```

Open `http://localhost:5173` in your browser.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

This project would not be possible without the excellent work of:

- **[FluentValidation](https://github.com/FluentValidation/FluentValidation)** - A powerful .NET validation library created by [Jeremy Skinner](https://github.com/JeremySkinner). FluentValidation provides a fluent interface for building strongly-typed validation rules in C#.

- **[fluentvalidation-ts](https://github.com/AlexJPotter/fluentvalidation-ts)** - A TypeScript validation library created by [Alex Potter](https://github.com/AlexJPotter), inspired by FluentValidation. This brings the same elegant validation syntax to TypeScript/JavaScript applications.

Special thanks to both Jeremy and Alex for creating and maintaining these foundational libraries that make FluentValidationCLI possible.

## 📧 Contact

For questions or support, please open an issue on GitHub.

---

**Built with ❤️ to eliminate redundant validation code**
