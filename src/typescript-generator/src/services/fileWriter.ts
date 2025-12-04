import * as fs from 'fs/promises';
import * as path from 'path';

/**
 * Handles writing generated code to the file system
 */
export class FileWriter {
  /**
   * Writes generated validator code to a file
   */
  async writeFile(outputPath: string, fileName: string, content: string): Promise<void> {
    // Create output directory if it doesn't exist
    try {
      await fs.access(outputPath);
    } catch {
      await fs.mkdir(outputPath, { recursive: true });
      console.log(`Created output directory: ${outputPath}`);
    }

    const fullPath = path.join(outputPath, fileName);

    // Write the file
    await fs.writeFile(fullPath, content, 'utf-8');

    console.log(`✓ Generated ${fileName}`);
  }

  /**
   * Writes multiple validator files
   */
  async writeFiles(outputPath: string, files: Map<string, string>): Promise<void> {
    for (const [fileName, content] of files.entries()) {
      await this.writeFile(outputPath, fileName, content);
    }
  }
}
