/**
 * Root model representing a complete validation definition for an entity
 */
export interface ValidationDefinition {
  /**
   * Name of the entity being validated (e.g., "User", "Product")
   */
  entity: string;

  /**
   * Namespace for the generated validator (optional for TypeScript)
   */
  namespace: string;

  /**
   * List of properties with their validation rules
   */
  properties: PropertyValidation[];
}

/**
 * Validation definition for a single property
 */
export interface PropertyValidation {
  /**
   * Name of the property (e.g., "email", "age")
   */
  name: string;

  /**
   * Data type of the property (string, number, boolean, date)
   */
  type: 'string' | 'number' | 'boolean' | 'date';

  /**
   * List of validation rules to apply to this property
   */
  rules: ValidationRule[];
}

/**
 * Individual validation rule
 */
export interface ValidationRule {
  /**
   * Name of the validator (e.g., "notEmpty", "emailAddress", "length")
   */
  validator: string;

  /**
   * Parameters for the validator (e.g., min/max for range validators)
   */
  parameters?: Record<string, any>;

  /**
   * Custom error message for this validation rule
   */
  message?: string;

  /**
   * Conditional expression for when this rule should apply (future feature)
   */
  when?: string;
}
