# Walkthrough - Centralized Validation Rules Engine

I have successfully implemented the Centralized Validation Rules Engine. This system allows you to define validation rules in a JSON format and automatically generate C# (FluentValidation) and TypeScript (fluentvalidation-ts) validation classes.

## Project Structure

- `rules/`: Contains the JSON rule definitions (e.g., `User.json`).
- `src/RuleMaker.Generator`: Console application that parses JSON rules and generates code.
- `src/RuleMaker.Backend`: ASP.NET Core Web API sample using the generated C# validators.
- `src/RuleMaker.Frontend`: React/TypeScript sample using the generated TypeScript validators.

## How to Use

### 1. Define Rules

Create or edit JSON files in the `rules/` directory.
**Example `User.json`:**

```json
{
  "entityName": "User",
  "properties": [
    {
      "name": "Username",
      "type": "string",
      "rules": [{ "type": "NotEmpty", "message": "Username is required" }]
    }
  ]
}
```

### 2. Run Generator

Run the generator to update the validation code in both backend and frontend projects.

```bash
dotnet run --project src/RuleMaker.Generator/RuleMaker.Generator.csproj
```

### 3. Run Backend

Start the ASP.NET Core API.

```bash
dotnet run --project src/RuleMaker.Backend/RuleMaker.Backend.csproj
```

The API has a `POST /validate-user` endpoint.

### 4. Run Frontend

Start the React app.

```bash
cd src/RuleMaker.Frontend
npm run dev
```

The app provides a form to test validation in real-time.

## Verification Results

### Generator

- Successfully parsed `User.json`.
- Generated `UserValidator.cs` in `src/RuleMaker.Backend/Validators`.
- Generated `UserValidator.ts` in `src/RuleMaker.Frontend/src/validators`.

### Backend

- Built successfully.
- `UserValidator` is registered and used in `Program.cs`.

### Frontend

- Built successfully.
- `UserValidator` is used in `App.tsx` to validate form state.

## Next Steps

- Add support for more validation rules (Regex, Custom, etc.).
- Improve the CLI to handle more configuration options.
- Integrate into a CI/CD pipeline to run the generator automatically.
