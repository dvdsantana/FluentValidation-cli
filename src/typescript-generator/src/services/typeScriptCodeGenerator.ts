import { ValidationDefinition, PropertyValidation } from '../models/validationRuleModels';
import { ValidatorMapping } from '../mapping/validatorMapping';

/**
 * Generates TypeScript fluentvalidation-ts validator classes from ValidationDefinitions
 */
export class TypeScriptCodeGenerator {
  private readonly indentSize = 2;

  /**
   * Generates TypeScript validator code for a single entity
   */
  generateValidator(definition: ValidationDefinition): string {
    const lines: string[] = [];

    // Add import statement
    lines.push("import { Validator } from 'fluentvalidation-ts';");
    lines.push('');

    // Generate type definition
    lines.push(this.generateTypeDefinition(definition));
    lines.push('');

    // Generate validator class
    const className = `${definition.entity}Validator`;
    lines.push(`export class ${className} extends Validator<${definition.entity}> {`);
    lines.push(`${this.indent(1)}constructor() {`);
    lines.push(`${this.indent(2)}super();`);

    // Generate rules for each property
    for (const property of definition.properties) {
      this.generatePropertyRules(lines, property);
    }

    // Close constructor
    lines.push(`${this.indent(1)}}`);

    // Close class
    lines.push('}');

    return lines.join('\n');
  }

  /**
   * Generates the TypeScript type definition for the entity
   */
  private generateTypeDefinition(definition: ValidationDefinition): string {
    const lines: string[] = [];

    lines.push(`export type ${definition.entity} = {`);

    for (const property of definition.properties) {
      const tsType = this.mapToTypeScriptType(property.type);
      lines.push(`${this.indent(1)}${this.toCamelCase(property.name)}: ${tsType};`);
    }

    lines.push('};');

    return lines.join('\n');
  }

  /**
   * Maps JSON type names to TypeScript types
   */
  private mapToTypeScriptType(type: string): string {
    switch (type.toLowerCase()) {
      case 'string':
        return 'string';
      case 'number':
        return 'number';
      case 'boolean':
        return 'boolean';
      case 'date':
        return 'Date';
      default:
        return 'any';
    }
  }

  /**
   * Converts property name to camelCase for TypeScript convention
   */
  private toCamelCase(str: string): string {
    return str.charAt(0).toLowerCase() + str.slice(1);
  }

  /**
   * Generates validation rules for a single property
   */
  private generatePropertyRules(lines: string[], property: PropertyValidation): void {
    if (property.rules.length === 0) {
      return;
    }

    lines.push('');

    // Start ruleFor chain
    const propName = this.toCamelCase(property.name);
    lines.push(`${this.indent(2)}this.ruleFor('${propName}')`);

    // Add each validation rule in the chain
    for (const rule of property.rules) {
      const methodCall = ValidatorMapping.mapToTypeScript(rule.validator, rule.parameters);
      lines.push(`${this.indent(3)}.${methodCall}`);

      // Add custom message if provided
      if (rule.message) {
        const escapedMessage = this.escapeString(rule.message);
        lines.push(`${this.indent(3)}.withMessage('${escapedMessage}')`);
      }
    }

    // Remove the last character (which would be a closing paren) and add semicolon
    const lastLine = lines[lines.length - 1];
    lines[lines.length - 1] = lastLine + ';';
  }

  /**
   * Creates indentation string
   */
  private indent(level: number): string {
    return ' '.repeat(level * this.indentSize);
  }

  /**
   * Escapes special characters in strings for TypeScript code
   */
  private escapeString(input: string): string {
    return input
      .replace(/\\/g, '\\\\')
      .replace(/'/g, "\\'")
      .replace(/\n/g, '\\n')
      .replace(/\r/g, '\\r')
      .replace(/\t/g, '\\t');
  }

  /**
   * Generates the filename for the validator
   */
  getValidatorFileName(definition: ValidationDefinition): string {
    return `${definition.entity}Validator.ts`;
  }
}
