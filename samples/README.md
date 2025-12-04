# Sample Applications - Quick Start Guide

This directory contains sample applications demonstrating the generated validators in action.

## Backend (ASP.NET Core Web API)

### Running the Backend

```bash
cd samples/backend
dotnet run
```

The API will be available at:

- HTTPS: `https://localhost:7139`
- Swagger UI: `https://localhost:7139/swagger`

### Available Endpoints

**Users:**

- `POST /api/users` - Create a new user (with validation)
- `GET /api/users/{id}` - Get a user by ID
- `POST /api/users/validate` - Validate user data without saving

**Products:**

- `POST /api/products` - Create a new product (with validation)
- `GET /api/products/{id}` - Get a product by ID
- `POST /api/products/validate` - Validate product data without saving

### Testing with curl

**Valid User:**

```bash
curl -X POST https://localhost:7139/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "age": 25,
    "name": "John Doe"
  }' \
  -k
```

**Invalid User (should return validation errors):**

```bash
curl -X POST https://localhost:7139/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "invalid-email",
    "age": 15,
    "name": "A"
  }' \
  -k
```

**Valid Product:**

```bash
curl -X POST https://localhost:7139/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Premium Widget",
    "price": 49.99,
    "sku": "ABC-1234",
    "description": "A high-quality widget"
  }' \
  -k
```

---

## Frontend (React + TypeScript)

### Running the Frontend

```bash
cd samples/frontend
npm install  # if not already done
npm run dev
```

The application will be available at `http://localhost:5173`

### Features

- **User Registration Form**: Validates email, age (18-120), and name (2-100 chars)
- **Product Creation Form**: Validates SKU format (XXX-9999), price (0-100,000), etc.
- **Real-time Validation**: Errors display as you type
- **Backend Integration**: Submits to the ASP.NET Core API

### Testing the Frontend

1. Open `http://localhost:5173` in your browser
2. Try the User Registration tab:
   - Leave fields empty → See "required" errors
   - Enter invalid email → See "valid email" error
   - Enter age < 18 → See age range error
3. Try the Product Creation tab:
   - Enter invalid SKU → See format error
   - Enter price > 100,000 → See price limit error

---

## How the Validation Works

### 1. JSON Definition

Validation rules are defined once in JSON:

```json
{
  "entity": "User",
  "properties": [
    {
      "name": "Email",
      "rules": [
        {
          "validator": "EmailAddress",
          "message": "Please provide a valid email address"
        }
      ]
    }
  ]
}
```

### 2. C# Generation

Run the generator:

```bash
dotnet run --project ../../src/csharp-generator -- generate --input ../../rules --output ./Validators
```

This creates `UserValidator.cs`:

```csharp
RuleFor(x => x.Email)
    .EmailAddress()
    .WithMessage("Please provide a valid email address");
```

### 3. TypeScript Generation

Run the generator:

```bash
npm run dev --prefix ../../src/typescript-generator -- generate --input ../../rules --output ./src/validators
```

This creates `UserValidator.ts`:

```typescript
this.ruleFor("email")
  .emailAddress()
  .withMessage("Please provide a valid email address");
```

### 4. Usage

**Backend (C#):**

```csharp
var validationResult = await _validator.ValidateAsync(user);
if (!validationResult.IsValid) {
    return ValidationProblem(validationResult.ToDictionary());
}
```

**Frontend (TypeScript):**

```typescript
const validationErrors = userValidator.validate(formData);
if (Object.keys(validationErrors).length > 0) {
  setErrors(validationErrors);
  return;
}
```

---

## Benefits Demonstrated

✅ **Single Source of Truth**: Rules defined once in JSON  
✅ **Identical Logic**: Same validation on frontend and backend  
✅ **Type Safety**: Fully typed in both C# and TypeScript  
✅ **DRY Principle**: No duplicate validation code  
✅ **Maintainability**: Change JSON, regenerate, done!

---

## Troubleshooting

### Backend won't start

- Make sure .NET 8 SDK is installed: `dotnet --version`
- Check if port 7139 is available
- Run `dotnet build` to check for errors

### Frontend won't start

- Make sure Node.js is installed: `node --version`
- Delete `node_modules` and run `npm install` again
- Check if port 5173 is available

### CORS errors

- Make sure the backend is running first
- Check that the API URL in the frontend matches the backend port
- The backend has CORS enabled for all origins in development

### Validation not working

- Verify validators are copied to the correct directories
- Check browser console for JavaScript errors
- Check backend logs for validation errors
