# Centralized Validation Rules Engine - Task Breakdown

## Phase 1: JSON Schema Design & Validation Rule Definitions

- [x] Design JSON schema structure for validation rules
  - [x] Define entity-level structure (e.g., User.json, Product.json)
  - [x] Define property-level validation rules schema
  - [x] Map common validation types to both FluentValidation and fluentvalidation-ts
  - [x] Support for basic validators (NotNull, NotEmpty, Length, Email, etc.)
  - [x] Support for numeric validators (GreaterThan, LessThan, Between, etc.)
  - [x] Support for string validators (Regex, MinLength, MaxLength, etc.)
  - [x] Support for custom error messages
  - [x] Support for conditional validation rules (When/Unless)
- [x] Create JSON schema documentation
- [x] Create sample entity validation files (User.json, Address.json, Product.json)

## Phase 2: C# FluentValidation Code Generator

- [x] Set up .NET project structure
  - [x] Create CLI tool project
  - [x] Add FluentValidation package references
  - [x] Set up project dependencies and configuration
- [x] Implement JSON parser for validation rules
  - [x] Read and deserialize JSON validation files
  - [x] Validate JSON structure against schema
  - [x] Create domain models for validation rules
- [x] Implement C# code generator
  - [x] Create code generation engine
  - [x] Generate AbstractValidator<T> class structure
  - [x] Map JSON rules to FluentValidation methods
  - [x] Generate RuleFor statements
  - [x] Handle custom error messages (WithMessage)
  - [/] Handle conditional rules (When/Unless)
  - [x] Generate proper namespaces and using statements
- [x] Implement file output system
  - [x] Write generated C# files to output directory
  - [x] Handle file naming conventions
  - [x] Support for overwrite/merge options
- [ ] Add unit tests for C# generator

## Phase 3: TypeScript fluentvalidation-ts Code Generator

- [x] Set up TypeScript/Node.js project structure
  - [x] Create CLI tool project
  - [x] Add fluentvalidation-ts package references
  - [x] Set up TypeScript compilation configuration
- [x] Implement JSON parser for validation rules (TypeScript version)
  - [x] Read and parse JSON validation files
  - [x] Validate JSON structure
  - [x] Create TypeScript interfaces for validation rules
- [x] Implement TypeScript code generator

# Centralized Validation Rules Engine - Task Breakdown

## Phase 1: JSON Schema Design & Validation Rule Definitions

- [x] Design JSON schema structure for validation rules
  - [x] Define entity-level structure (e.g., User.json, Product.json)
  - [x] Define property-level validation rules schema
  - [x] Map common validation types to both FluentValidation and fluentvalidation-ts
  - [x] Support for basic validators (NotNull, NotEmpty, Length, Email, etc.)
  - [x] Support for numeric validators (GreaterThan, LessThan, Between, etc.)
  - [x] Support for string validators (Regex, MinLength, MaxLength, etc.)
  - [x] Support for custom error messages
  - [x] Support for conditional validation rules (When/Unless)
- [x] Create JSON schema documentation
- [x] Create sample entity validation files (User.json, Address.json, Product.json)

## Phase 2: C# FluentValidation Code Generator

- [x] Set up .NET project structure
  - [x] Create CLI tool project
  - [x] Add FluentValidation package references
  - [x] Set up project dependencies and configuration
- [x] Implement JSON parser for validation rules
  - [x] Read and deserialize JSON validation files
  - [x] Validate JSON structure against schema
  - [x] Create domain models for validation rules
- [x] Implement C# code generator
  - [x] Create code generation engine
  - [x] Generate AbstractValidator<T> class structure
  - [x] Map JSON rules to FluentValidation methods
  - [x] Generate RuleFor statements
  - [x] Handle custom error messages (WithMessage)
  - [/] Handle conditional rules (When/Unless)
  - [x] Generate proper namespaces and using statements
- [x] Implement file output system
  - [x] Write generated C# files to output directory
  - [x] Handle file naming conventions
  - [x] Support for overwrite/merge options
- [ ] Add unit tests for C# generator

## Phase 3: TypeScript fluentvalidation-ts Code Generator

- [x] Set up TypeScript/Node.js project structure
  - [x] Create CLI tool project
  - [x] Add fluentvalidation-ts package references
  - [x] Set up TypeScript compilation configuration
- [x] Implement JSON parser for validation rules (TypeScript version)
  - [x] Read and parse JSON validation files
  - [x] Validate JSON structure
  - [x] Create TypeScript interfaces for validation rules
- [x] Implement TypeScript code generator
  - [x] Create code generation engine
  - [x] Generate Validator<TModel> class structure
  - [x] Map JSON rules to fluentvalidation-ts methods
  - [x] Generate ruleFor statements
  - [x] Handle custom error messages (withMessage)
  - [/] Handle conditional rules (when/unless)
  - [x] Generate proper imports and type definitions
- [x] Implement file output system
  - [x] Write generated TypeScript files to output directory

## Phase 5: Documentation & Build Pipeline

- [x] Create user documentation
  - [x] Getting started guide
  - [x] JSON schema reference
  - [x] C# generator usage guide
  - [x] TypeScript generator usage guide
  - [x] Sample applications guide
  - [x] Troubleshooting guide
- [x] Create developer documentation
  - [x] Architecture overview
  - [x] Code generation process
  - [x] Extension points
  - [/] Contributing guide
- [x] Set up build pipeline integration
  - [x] Create pre-build hooks for C# projects
  - [x] Create npm scripts for TypeScript projects
  - [x] Document CI/CD integration
  - [x] Create example build scripts
- [x] Create README files for all projects
- [x] Add licensing and contribution guidelines
