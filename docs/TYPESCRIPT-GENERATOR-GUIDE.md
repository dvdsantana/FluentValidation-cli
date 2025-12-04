# TypeScript Generator Usage Guide

Complete guide for using the TypeScript fluentvalidation-ts code generator.

## Installation

### Global Installation (npm)

```bash
cd src/typescript-generator
npm install -g .
```

Or from npm registry (if published):

```bash
npm install -g @fluentvalidation/typescript-generator
```

### Local Installation

```bash
npm install --save-dev @fluentvalidation/typescript-generator
```

### Build from Source

```bash
cd src/typescript-generator
npm install
npm run build
```

## Usage

### Basic Command

```bash
fv-ts-generator generate --input ./rules --output ./src/validators
```

### Command Options

```
fv-ts-generator generate [options]

Options:
  -i, --input <path>    Path to directory containing JSON rule files
                        Default: ./rules

  -o, --output <path>   Path to directory for generated TypeScript files
                        Default: ./validators

  -h, --help           Display help information
  -V, --version        Display version information
```

### Examples

**Generate with custom paths:**

```bash
fv-ts-generator generate \
  --input ./ValidationRules \
  --output ./src/app/validators
```

**Using short options:**

```bash
fv-ts-generator generate -i ./rules -o ./validators
```

**Via npx (no installation):**

```bash
npx fv-ts-generator generate -i ./rules -o ./validators
```

---

## Integration with React

### Step 1: Generate Validators

```bash
fv-ts-generator generate \
  --input ./rules \
  --output ./src/validators
```

### Step 2: Install fluentvalidation-ts

```bash
npm install fluentvalidation-ts
```

### Step 3: Use in Components

**UserForm.tsx:**

```typescript
import { useState } from "react";
import { UserValidator } from "../validators/UserValidator";
import type { User } from "../validators/UserValidator";

const userValidator = new UserValidator();

export function UserForm() {
  const [formData, setFormData] = useState<Partial<User>>({});
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // Validate using generated validator
    const validationErrors = userValidator.validate(formData as User);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    // Submit form...
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        value={formData.email || ""}
        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
      />
      {errors.email && <span>{errors.email}</span>}

      <button type="submit">Submit</button>
    </form>
  );
}
```

---

## Integration with Vue

**UserForm.vue:**

```vue
<script setup lang="ts">
import { ref, reactive } from "vue";
import { UserValidator } from "../validators/UserValidator";
import type { User } from "../validators/UserValidator";

const userValidator = new UserValidator();
const formData = reactive<Partial<User>>({});
const errors = ref<Record<string, string>>({});

const handleSubmit = () => {
  const validationErrors = userValidator.validate(formData as User);

  if (Object.keys(validationErrors).length > 0) {
    errors.value = validationErrors;
    return;
  }

  // Submit form...
};
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <input v-model="formData.email" />
    <span v-if="errors.email">{{ errors.email }}</span>

    <button type="submit">Submit</button>
  </form>
</template>
```

---

## Integration with Angular

**user-form.component.ts:**

```typescript
import { Component } from "@angular/core";
import { UserValidator } from "../validators/UserValidator";
import type { User } from "../validators/UserValidator";

@Component({
  selector: "app-user-form",
  templateUrl: "./user-form.component.html",
})
export class UserFormComponent {
  private userValidator = new UserValidator();

  formData: Partial<User> = {};
  errors: Record<string, string> = {};

  onSubmit() {
    this.errors = this.userValidator.validate(this.formData as User);

    if (Object.keys(this.errors).length === 0) {
      // Submit form...
    }
  }
}
```

---

## Build Pipeline Integration

### package.json Scripts

Add to your `package.json`:

```json
{
  "scripts": {
    "generate:validators": "fv-ts-generator generate -i ./rules -o ./src/validators",
    "prebuild": "npm run generate:validators",
    "build": "vite build"
  }
}
```

Now validators regenerate automatically before each build:

```bash
npm run build
```

### Pre-commit Hook

**Using husky:**

```bash
npm install --save-dev husky
npx husky init
```

**.husky/pre-commit:**

```bash
#!/bin/sh
npm run generate:validators
git add src/validators/
```

### CI/CD Pipeline

**GitHub Actions:**

```yaml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: "18"

      - name: Install dependencies
        run: npm ci

      - name: Generate validators
        run: npm run generate:validators

      - name: Build
        run: npm run build

      - name: Test
        run: npm test
```

**GitLab CI:**

```yaml
build:
  image: node:18
  script:
    - npm ci
    - npm run generate:validators
    - npm run build
```

---

## Advanced Usage

### Watch Mode for Development

Create a watch script for automatic regeneration:

**package.json:**

```json
{
  "scripts": {
    "watch:rules": "nodemon --watch rules --ext json --exec 'npm run generate:validators'"
  }
}
```

**Install nodemon:**

```bash
npm install --save-dev nodemon
```

**Run watch mode:**

```bash
npm run watch:rules
```

Now validators regenerate whenever you change JSON files!

### Multiple Rule Directories

Generate validators from multiple sources:

**package.json:**

```json
{
  "scripts": {
    "generate:domain": "fv-ts-generator generate -i ./rules/domain -o ./src/validators/domain",
    "generate:app": "fv-ts-generator generate -i ./rules/app -o ./src/validators/app",
    "generate:all": "npm run generate:domain && npm run generate:app"
  }
}
```

### Custom Output Organization

Organize by feature:

```bash
# User feature
fv-ts-generator generate \
  -i ./rules/users \
  -o ./src/features/users/validators

# Product feature
fv-ts-generator generate \
  -i ./rules/products \
  -o ./src/features/products/validators
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

**Output (UserValidator.ts):**

```typescript
import { Validator } from "fluentvalidation-ts";

export type User = {
  email: string;
};

export class UserValidator extends Validator<User> {
  constructor() {
    super();

    this.ruleFor("email")
      .notEmpty()
      .withMessage("Email is required")
      .emailAddress();
  }
}
```

### File Naming Convention

- Generated files use the pattern: `{EntityName}Validator.ts`
- Examples: `UserValidator.ts`, `ProductValidator.ts`

### Type Definitions

The generator creates TypeScript type definitions for each entity:

```typescript
export type User = {
  id: number;
  email: string;
  age: number;
  name: string;
};
```

Property names are converted to camelCase automatically.

---

## Troubleshooting

### "Command not found: fv-ts-generator"

**Cause:** Tool not installed globally  
**Solution:**

```bash
npm install -g @fluentvalidation/typescript-generator
```

Or use npx:

```bash
npx fv-ts-generator generate ...
```

Or use local installation:

```bash
npm install --save-dev @fluentvalidation/typescript-generator
npx fv-ts-generator generate ...
```

### "Input directory not found"

**Cause:** Incorrect path to rules directory  
**Solution:** Use absolute path or verify relative path:

```bash
fv-ts-generator generate --input "/absolute/path/to/rules" --output "./validators"
```

### "Module 'fluentvalidation-ts' not found"

**Cause:** Missing dependency  
**Solution:**

```bash
npm install fluentvalidation-ts
```

### TypeScript compilation errors in generated files

**Cause:** Type mismatches or missing imports  
**Solutions:**

1. Ensure `fluentvalidation-ts` is installed
2. Check `tsconfig.json` includes the validators directory
3. Verify property types match your models

### Generated validators not updating

**Cause:** Old files cached  
**Solution:**

1. Delete `dist/` or `build/` directory
2. Regenerate validators
3. Rebuild project

---

## Tips & Best Practices

### 1. Type Safety

Export and use generated types:

```typescript
import type { User } from "./validators/UserValidator";

const user: User = {
  id: 1,
  email: "user@example.com",
  age: 25,
  name: "John",
};
```

### 2. Reuse Validators

Create a single instance per validator:

```typescript
// validators/index.ts
export const userValidator = new UserValidator();
export const productValidator = new ProductValidator();

// In components
import { userValidator } from "../validators";
```

### 3. Custom Error Handling

Transform validation errors to your preferred format:

```typescript
function formatErrors(errors: Record<string, string>) {
  return Object.entries(errors).map(([field, message]) => ({
    field,
    message,
    timestamp: new Date(),
  }));
}
```

### 4. Testing

Write unit tests for validators:

```typescript
import { describe, it, expect } from "vitest";
import { UserValidator } from "./UserValidator";

describe("UserValidator", () => {
  const validator = new UserValidator();

  it("should reject invalid email", () => {
    const errors = validator.validate({
      email: "invalid",
      age: 25,
      name: "John",
    });

    expect(errors.email).toBeDefined();
  });
});
```

### 5. Version Control

- Commit generated validators
- Include JSON rules in repository
- Review diffs when regenerating

---

## Support

For issues or questions:

- Check [JSON Schema Reference](JSON-SCHEMA-REFERENCE.md)
- Review [sample applications](../samples/README.md)
- Open an issue on GitHub
