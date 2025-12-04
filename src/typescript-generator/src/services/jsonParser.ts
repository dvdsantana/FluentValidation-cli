import * as fs from 'fs/promises';
import * as path from 'path';
import { ValidationDefinition } from '../models/validationRuleModels';

/**
 * Service for parsing and validating JSON rule definition files
 */
export class JsonParser {
  /**
   * Reads and parses all JSON files from the specified directory
   */
  async parseDirectory(inputPath: string): Promise<ValidationDefinition[]> {
    try {
      const stats = await fs.stat(inputPath);
      if (!stats.isDirectory()) {
        throw new Error(`Input path is not a directory: ${inputPath}`);
      }
    } catch (error) {
      throw new Error(`Input directory not found: ${inputPath}`);
    }

    const files = await fs.readdir(inputPath);
    const jsonFiles = files.filter(file => file.endsWith('.json'));

    if (jsonFiles.length === 0) {
      console.log(`Warning: No JSON files found in ${inputPath}`);
      return [];
    }

    const definitions: ValidationDefinition[] = [];

    for (const fileName of jsonFiles) {
      try {
        const filePath = path.join(inputPath, fileName);
        const definition = await this.parseFile(filePath);
        definitions.push(definition);
        console.log(`✓ Parsed ${fileName}`);
      } catch (error) {
        console.error(`✗ Error parsing ${fileName}: ${(error as Error).message}`);
        throw error;
      }
    }

    return definitions;
  }

  /**
   * Parses a single JSON file into a ValidationDefinition
   */
  async parseFile(filePath: string): Promise<ValidationDefinition> {
    const fileContent = await fs.readFile(filePath, 'utf-8');
    const definition = JSON.parse(fileContent) as ValidationDefinition;

    this.validateDefinition(definition, filePath);

    return definition;
  }

  /**
   * Validates that the parsed definition has all required fields
   */
  private validateDefinition(definition: ValidationDefinition, filePath: string): void {
    const errors: string[] = [];

    if (!definition.entity || definition.entity.trim() === '') {
      errors.push('Entity name is required');
    }

    if (!definition.namespace || definition.namespace.trim() === '') {
      errors.push('Namespace is required');
    }

    if (!definition.properties || definition.properties.length === 0) {
      errors.push('At least one property must be defined');
    } else {
      definition.properties.forEach((prop, i) => {
        if (!prop.name || prop.name.trim() === '') {
          errors.push(`Property[${i}]: Name is required`);
        }

        if (!prop.type || prop.type.trim() === '') {
          errors.push(`Property[${i}] (${prop.name}): Type is required`);
        }

        if (!prop.rules || prop.rules.length === 0) {
          errors.push(`Property[${i}] (${prop.name}): At least one validation rule is required`);
        } else {
          prop.rules.forEach((rule, j) => {
            if (!rule.validator || rule.validator.trim() === '') {
              errors.push(`Property[${i}] (${prop.name}), Rule[${j}]: Validator name is required`);
            }
          });
        }
      });
    }

    if (errors.length > 0) {
      throw new Error(
        `Validation errors in ${path.basename(filePath)}:\n  - ${errors.join('\n  - ')}`
      );
    }
  }
}
