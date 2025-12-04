import { ValidationRule } from '../models/validationRuleModels';

/**
 * Maps JSON validator names to TypeScript fluentvalidation-ts method calls
 */
export class ValidatorMapping {
  /**
   * Generates the TypeScript fluentvalidation-ts method call for a given validator and its parameters
   */
  static mapToTypeScript(validatorName: string, parameters?: Record<string, any>): string {
    switch (validatorName) {
      case 'NotNull':
        return 'notNull()';
      case 'NotEmpty':
        return 'notEmpty()';
      case 'Empty':
        return 'empty()';

      case 'Equal':
        return this.generateEqual(parameters);
      case 'NotEqual':
        return this.generateNotEqual(parameters);

      case 'Length':
        return this.generateLength(parameters);
      case 'MinLength':
        return this.generateMinLength(parameters);
      case 'MaxLength':
        return this.generateMaxLength(parameters);

      case 'EmailAddress':
        return 'emailAddress()';

      case 'Matches':
        return this.generateMatches(parameters);

      case 'LessThan':
        return this.generateLessThan(parameters);
      case 'LessThanOrEqualTo':
        return this.generateLessThanOrEqualTo(parameters);
      case 'GreaterThan':
        return this.generateGreaterThan(parameters);
      case 'GreaterThanOrEqualTo':
        return this.generateGreaterThanOrEqualTo(parameters);

      case 'InclusiveBetween':
        return this.generateInclusiveBetween(parameters);
      case 'ExclusiveBetween':
        return this.generateExclusiveBetween(parameters);

      default:
        throw new Error(`Validator '${validatorName}' is not supported`);
    }
  }

  private static generateEqual(parameters?: Record<string, any>): string {
    const value = this.getRequiredParameter(parameters, 'value', 'Equal');
    return `equal(${this.formatValue(value)})`;
  }

  private static generateNotEqual(parameters?: Record<string, any>): string {
    const value = this.getRequiredParameter(parameters, 'value', 'NotEqual');
    return `notEqual(${this.formatValue(value)})`;
  }

  private static generateLength(parameters?: Record<string, any>): string {
    const min = this.getRequiredParameter(parameters, 'min', 'Length');
    const max = this.getRequiredParameter(parameters, 'max', 'Length');
    return `length(${min}, ${max})`;
  }

  private static generateMinLength(parameters?: Record<string, any>): string {
    const length = this.getRequiredParameter(parameters, 'length', 'MinLength');
    return `minLength(${length})`;
  }

  private static generateMaxLength(parameters?: Record<string, any>): string {
    const length = this.getRequiredParameter(parameters, 'length', 'MaxLength');
    return `maxLength(${length})`;
  }

  private static generateMatches(parameters?: Record<string, any>): string {
    const pattern = this.getRequiredParameter(parameters, 'pattern', 'Matches');
    // Escape backslashes for regex pattern in TypeScript
    const escapedPattern = String(pattern).replace(/\\/g, '\\\\');
    return `matches(new RegExp('${escapedPattern}'))`;
  }

  private static generateLessThan(parameters?: Record<string, any>): string {
    const value = this.getRequiredParameter(parameters, 'value', 'LessThan');
    return `lessThan(${value})`;
  }

  private static generateLessThanOrEqualTo(parameters?: Record<string, any>): string {
    const value = this.getRequiredParameter(parameters, 'value', 'LessThanOrEqualTo');
    return `lessThanOrEqualTo(${value})`;
  }

  private static generateGreaterThan(parameters?: Record<string, any>): string {
    const value = this.getRequiredParameter(parameters, 'value', 'GreaterThan');
    return `greaterThan(${value})`;
  }

  private static generateGreaterThanOrEqualTo(parameters?: Record<string, any>): string {
    const value = this.getRequiredParameter(parameters, 'value', 'GreaterThanOrEqualTo');
    return `greaterThanOrEqualTo(${value})`;
  }

  private static generateInclusiveBetween(parameters?: Record<string, any>): string {
    const min = this.getRequiredParameter(parameters, 'min', 'InclusiveBetween');
    const max = this.getRequiredParameter(parameters, 'max', 'InclusiveBetween');
    return `inclusiveBetween(${min}, ${max})`;
  }

  private static generateExclusiveBetween(parameters?: Record<string, any>): string {
    const min = this.getRequiredParameter(parameters, 'min', 'ExclusiveBetween');
    const max = this.getRequiredParameter(parameters, 'max', 'ExclusiveBetween');
    return `exclusiveBetween(${min}, ${max})`;
  }

  private static getRequiredParameter(
    parameters: Record<string, any> | undefined,
    paramName: string,
    validatorName: string
  ): any {
    if (!parameters || !(paramName in parameters)) {
      throw new Error(`Validator '${validatorName}' requires parameter '${paramName}'`);
    }
    return parameters[paramName];
  }

  private static formatValue(value: any): string {
    if (typeof value === 'string') {
      return `'${value}'`;
    } else if (typeof value === 'boolean') {
      return value.toString();
    } else {
      return String(value);
    }
  }
}
