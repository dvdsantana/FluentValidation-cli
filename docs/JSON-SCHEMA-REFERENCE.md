# JSON Schema Reference

Complete reference for defining validation rules in JSON format.

## Schema Structure

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
          "validator": "ValidatorName",
          "parameters": {},
          "message": "Custom error message"
        }
      ]
    }
  ]
}
```

## Root Object Properties

### `entity` (required)

**Type:** `string`  
**Description:** Name of the entity being validated (e.g., "User", "Product")  
**Example:** `"entity": "User"`

### `namespace` (required)

**Type:** `string`  
**Description:** Namespace for C# classes or module path for TypeScript  
**Example:** `"namespace": "MyApp.Models"`

### `properties` (required)

**Type:** `array`  
**Description:** Array of property validation definitions

---

## Property Object

### `name` (required)

**Type:** `string`  
**Description:** Property name (PascalCase for C#, will be converted to camelCase for TypeScript)  
**Example:** `"name": "Email"`

### `type` (required)

**Type:** `string`  
**Allowed values:** `"string"`, `"number"`, `"boolean"`, `"date"`  
**Description:** Data type of the property  
**Example:** `"type": "string"`

### `rules` (required)

**Type:** `array`  
**Description:** Array of validation rules to apply

---

## Validation Rule Object

### `validator` (required)

**Type:** `string`  
**Description:** Name of the validator to apply  
**Example:** `"validator": "NotEmpty"`

### `parameters` (optional)

**Type:** `object`  
**Description:** Parameters for the validator  
**Example:** `"parameters": { "length": 100 }`

### `message` (optional)

**Type:** `string`  
**Description:** Custom error message for this rule  
**Example:** `"message": "Email is required"`

### `when` (optional, future feature)

**Type:** `string`  
**Description:** Conditional expression for when this rule should apply

---

## Supported Validators

### String Validators

#### `NotEmpty`

Ensures the string is not null, empty, or whitespace.

**Parameters:** None

**Example:**

```json
{
  "validator": "NotEmpty",
  "message": "Field is required"
}
```

**Generated C#:**

```csharp
.NotEmpty()
.WithMessage("Field is required")
```

**Generated TypeScript:**

```typescript
.notEmpty()
.withMessage('Field is required')
```

---

#### `EmailAddress`

Validates that the string is a valid email address.

**Parameters:** None

**Example:**

```json
{
  "validator": "EmailAddress",
  "message": "Please provide a valid email address"
}
```

---

#### `Length`

Ensures string length is between min and max (inclusive).

**Parameters:**

- `min` (number, required): Minimum length
- `max` (number, required): Maximum length

**Example:**

```json
{
  "validator": "Length",
  "parameters": { "min": 2, "max": 100 },
  "message": "Must be between 2 and 100 characters"
}
```

**Generated C#:**

```csharp
.Length(2, 100)
.WithMessage("Must be between 2 and 100 characters")
```

---

#### `MinLength`

Ensures string length is at least the specified value.

**Parameters:**

- `length` (number, required): Minimum length

**Example:**

```json
{
  "validator": "MinLength",
  "parameters": { "length": 5 },
  "message": "Must be at least 5 characters"
}
```

**Generated C#:**

```csharp
.MinimumLength(5)
```

---

#### `MaxLength`

Ensures string length does not exceed the specified value.

**Parameters:**

- `length` (number, required): Maximum length

**Example:**

```json
{
  "validator": "MaxLength",
  "parameters": { "length": 255 },
  "message": "Cannot exceed 255 characters"
}
```

---

#### `Matches`

Validates that the string matches a regular expression pattern.

**Parameters:**

- `pattern` (string, required): Regular expression pattern

**Example:**

```json
{
  "validator": "Matches",
  "parameters": { "pattern": "^[A-Z]{3}-\\d{4}$" },
  "message": "SKU must follow format: XXX-9999"
}
```

**Note:** Use double backslashes (`\\`) for escape sequences in JSON.

---

### Numeric Validators

#### `GreaterThan`

Ensures the number is strictly greater than the specified value.

**Parameters:**

- `value` (number, required): Threshold value

**Example:**

```json
{
  "validator": "GreaterThan",
  "parameters": { "value": 0 },
  "message": "Must be greater than 0"
}
```

---

#### `GreaterThanOrEqualTo`

Ensures the number is greater than or equal to the specified value.

**Parameters:**

- `value` (number, required): Threshold value

**Example:**

```json
{
  "validator": "GreaterThanOrEqualTo",
  "parameters": { "value": 18 },
  "message": "Must be at least 18"
}
```

---

#### `LessThan`

Ensures the number is strictly less than the specified value.

**Parameters:**

- `value` (number, required): Threshold value

---

#### `LessThanOrEqualTo`

Ensures the number is less than or equal to the specified value.

**Parameters:**

- `value` (number, required): Threshold value

---

#### `InclusiveBetween`

Ensures the number is between min and max (inclusive).

**Parameters:**

- `min` (number, required): Minimum value
- `max` (number, required): Maximum value

**Example:**

```json
{
  "validator": "InclusiveBetween",
  "parameters": { "min": 0, "max": 100 },
  "message": "Must be between 0 and 100"
}
```

**Generated C#:**

```csharp
.InclusiveBetween(0, 100)
```

**Generated TypeScript:**

```typescript
.inclusiveBetween(0, 100)
```

---

#### `ExclusiveBetween`

Ensures the number is between min and max (exclusive).

**Parameters:**

- `min` (number, required): Lower bound
- `max` (number, required): Upper bound

---

### General Validators

#### `NotNull`

Ensures the value is not null.

**Parameters:** None

**Example:**

```json
{
  "validator": "NotNull",
  "message": "Value is required"
}
```

---

#### `Equal`

Ensures the value equals the specified value.

**Parameters:**

- `value` (any, required): Value to compare against

**Example:**

```json
{
  "validator": "Equal",
  "parameters": { "value": true },
  "message": "Must accept terms"
}
```

---

#### `NotEqual`

Ensures the value does not equal the specified value.

**Parameters:**

- `value` (any, required): Value to compare against

---

#### `Empty`

Opposite of NotEmpty - ensures the value is null, empty, or the default value.

**Parameters:** None

---

#### `Null`

Opposite of NotNull - ensures the value is null.

**Parameters:** None

---

#### `CreditCard`

Validates that the string could be a valid credit card number.

**Parameters:** None

---

#### `IsInEnum`

Checks whether a numeric value is valid for an enum type.

**Parameters:** None

**Note:** This validator is primarily useful for C# where enums can be cast from invalid numeric values.

---

## Complete Examples

### User Entity

```json
{
  "entity": "User",
  "namespace": "MyApp.Models",
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
    }
  ]
}
```

### Product Entity with Regex

```json
{
  "entity": "Product",
  "namespace": "MyApp.Models",
  "properties": [
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
    }
  ]
}
```

---

## Best Practices

### Naming Conventions

- Use PascalCase for entity and property names in JSON
- C# generator preserves PascalCase
- TypeScript generator converts to camelCase automatically

### Error Messages

- Be specific and user-friendly
- Include expected format or range in the message
- Avoid technical jargon

### Rule Ordering

- Order rules from most to least important
- Put `NotNull`/`NotEmpty` first
- Format/pattern rules after basic checks

### Example:

```json
{
  "rules": [
    { "validator": "NotEmpty", "message": "Email is required" },
    { "validator": "EmailAddress", "message": "Invalid email format" },
    { "validator": "MaxLength", "parameters": { "length": 255 } }
  ]
}
```

---

## Troubleshooting

### Common Errors

**Error: "Entity name is required"**  
Solution: Add the `entity` property to your JSON file.

**Error: "Validator 'X' requires parameter 'Y'"**  
Solution: Add the required parameter to the `parameters` object.

**Error: "At least one validation rule is required"**  
Solution: Add at least one rule to the `rules` array for each property.

**Regex pattern not matching**  
Solution: Use double backslashes (`\\`) for escape sequences in JSON.
