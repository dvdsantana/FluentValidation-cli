# Implementation Plan - Centralized Validation Rules Engine

This plan outlines the steps to build a system that generates validation code for both .NET (Backend) and TypeScript (Frontend) from a single JSON source of truth.

## User Review Required

> [!IMPORTANT] > **JSON Schema Definition**: The structure of the JSON rule definitions is critical. I have proposed a schema below. Please review if this covers your expected validation scenarios.
> **Tech Stack**: I am assuming a .NET 8 Console App for the generator, ASP.NET Core for the backend sample, and React with TypeScript for the frontend sample.

## Proposed Changes

### 1. Project Structure Setup

Create a new solution `RuleMaker.sln` with the following projects:

- `src/RuleMaker.Generator`: Console application to run the generation process.
- `src/RuleMaker.Backend`: Sample ASP.NET Core Web API to demonstrate usage.
- `src/RuleMaker.Frontend`: Sample React/TypeScript app to demonstrate usage.
- `rules/`: Directory to store the JSON validation rule definitions.

### 2. JSON Schema Design

Define the format for entity validation rules.
**Example `User.json`:**

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

### 3. Generator Implementation (`RuleMaker.Generator`)

Develop the core logic to parse JSON and generate code.

#### [NEW] `RuleMaker.Generator/Models/RuleDefinition.cs`

Classes to deserialize the JSON rules.

#### [NEW] `RuleMaker.Generator/Generators/CSharpGenerator.cs`

Logic to generate C# `AbstractValidator<T>` classes.

- Mapping JSON rules to FluentValidation methods (`RuleFor(x => x.Prop).NotEmpty()`, etc.)

#### [NEW] `RuleMaker.Generator/Generators/TypeScriptGenerator.cs`

Logic to generate TypeScript classes extending `Validator<T>`.

- Mapping JSON rules to `fluentvalidation-ts` methods.

#### [NEW] `RuleMaker.Generator/Program.cs`

CLI entry point to accept input directory (JSONs) and output directories (C# and TS).

### 4. Integration & Verification

#### Backend Integration

- Create a `User` class in `RuleMaker.Backend`.
- Run generator to produce `UserValidator.cs`.
- Register validator in `Program.cs`.
- Create an endpoint to validate `User` objects.

#### Frontend Integration

- Create a `User` type in `RuleMaker.Frontend`.
- Run generator to produce `UserValidator.ts`.
- Use the validator in a React form.

## Verification Plan

### Automated Tests

- Unit tests in `RuleMaker.Generator.Tests` to verify:
  - JSON parsing correctness.
  - C# code string generation matches expected output.
  - TypeScript code string generation matches expected output.

### Manual Verification

1. **Define Rules**: Create `rules/User.json`.
2. **Run Generator**: Execute `RuleMaker.Generator`.
3. **Backend Check**: Run `RuleMaker.Backend`, send valid/invalid requests, verify 400 Bad Request with correct messages.
4. **Frontend Check**: Run `RuleMaker.Frontend`, fill form, verify real-time validation feedback.
