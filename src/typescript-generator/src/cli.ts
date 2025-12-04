#!/usr/bin/env node

import { Command } from 'commander';
import * as path from 'path';
import { JsonParser } from './services/jsonParser';
import { TypeScriptCodeGenerator } from './services/typeScriptCodeGenerator';
import { FileWriter } from './services/fileWriter';

const program = new Command();

program
  .name('fv-ts-generator')
  .description('FluentValidation TypeScript Code Generator - Generate fluentvalidation-ts classes from JSON rule definitions')
  .version('1.0.0');

program
  .command('generate')
  .description('Generate TypeScript validator classes from JSON rule definitions')
  .option('-i, --input <path>', 'Path to directory containing JSON rule definition files', './rules')
  .option('-o, --output <path>', 'Path to directory where generated TypeScript files should be written', './validators')
  .action(async (options) => {
    await generateValidators(options.input, options.output);
  });

async function generateValidators(inputPath: string, outputPath: string): Promise<void> {
  try {
    console.log('╔════════════════════════════════════════════════════════════════╗');
    console.log('║    FluentValidation TypeScript Code Generator v1.0.0          ║');
    console.log('╚════════════════════════════════════════════════════════════════╝');
    console.log('');

    const fullInputPath = path.resolve(inputPath);
    const fullOutputPath = path.resolve(outputPath);

    console.log(`Input:  ${fullInputPath}`);
    console.log(`Output: ${fullOutputPath}`);
    console.log('');

    // Parse JSON files
    console.log('Parsing JSON rule definitions...');
    const parser = new JsonParser();
    const definitions = await parser.parseDirectory(fullInputPath);

    if (definitions.length === 0) {
      console.log('No validation definitions found. Exiting.');
      return;
    }

    console.log(`Found ${definitions.length} entity definition(s)`);
    console.log('');

    // Generate code
    console.log('Generating TypeScript validators...');
    const codeGenerator = new TypeScriptCodeGenerator();
    const fileWriter = new FileWriter();

    for (const definition of definitions) {
      const code = codeGenerator.generateValidator(definition);
      const fileName = codeGenerator.getValidatorFileName(definition);
      await fileWriter.writeFile(fullOutputPath, fileName, code);
    }

    console.log('');
    console.log('╔════════════════════════════════════════════════════════════════╗');
    console.log(`║  ✓ Successfully generated ${definitions.length} validator(s)`);
    console.log('╚════════════════════════════════════════════════════════════════╝');
  } catch (error) {
    console.log('');
    console.log('╔════════════════════════════════════════════════════════════════╗');
    console.log('║  ✗ Error occurred during code generation');
    console.log('╚════════════════════════════════════════════════════════════════╝');
    console.log('');
    console.log(`Error: ${(error as Error).message}`);
    console.log('');
    process.exit(1);
  }
}

program.parse(process.argv);
