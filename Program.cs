using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

class DirectoryTree
{
    private static StringBuilder outputBuilder = new StringBuilder();
    private static HashSet<string> excludedFolders = new HashSet<string>();

    static void Main(string[] args)
    {
        // No given params
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a directory path as an argument.");
            Console.WriteLine("Usage: dotnet run -- <directory_path> [output_filename] [-e excluded_folder1 excluded_folder2 ...]");
            return;
        }

        string rootPath = args[0];
        int excludeIndex = Array.IndexOf(args, "-e");
        string outputFile;

        // Handle exclude arguments
        if (excludeIndex != -1)
        {
            // If -e is provided, everything after it are excluded folders
            for (int i = excludeIndex + 1; i < args.Length; i++)
            {
                excludedFolders.Add(args[i].ToLower());
            }
            // Output file is between root path and -e flag (if provided)
            outputFile = excludeIndex > 1 ? args[1] : "directory-tree.txt";
        }
        else
        {
            // No exclusions, handle output file normally
            outputFile = args.Length > 1 ? args[1] : "directory-tree.txt";
        }

        // Input file does not exist
        if (!Directory.Exists(rootPath))
        {
            Console.WriteLine("Directory does not exist.");
            return;
        }

        string rootName = new DirectoryInfo(rootPath).Name;
        string rootLine = $"{rootName}/";
        Console.WriteLine(rootLine);
        outputBuilder.AppendLine(rootLine);

        if (excludedFolders.Count > 0)
        {
            Console.WriteLine("\nExcluded folders:");
            foreach (var folder in excludedFolders)
            {
                Console.WriteLine($"- {folder}");
            }
            Console.WriteLine();
        }

        DisplayDirectoryStructure(rootPath, "");

        // Write to file
        try
        {
            File.WriteAllText(outputFile, outputBuilder.ToString());
            Console.WriteLine($"\nTree structure has been saved to {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError writing to file: {ex.Message}");
        }
    }

    static void DisplayDirectoryStructure(string path, string indent)
    {
        try
        {
            // Get all the sub directories of the given directory
            var directories = Directory.GetDirectories(path);

            // Filter out excluded directories
            directories = directories.Where(d =>
                !excludedFolders.Contains(new DirectoryInfo(d).Name.ToLower())).ToArray();

            // Returns the names of files (including their paths) in the specified directory.
            var files = Directory.GetFiles(path);

            // Combine together
            var items = directories.Concat(files).ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                // Check whether we're at the last file of the directory
                bool isLast = (i == items.Length - 1);
                string connector = isLast ? "└── " : "├── ";
                string newIndent = indent + (isLast ? "    " : "│   ");

                // Determines whether the given path refers to an existing directory on disk.
                bool isDirectory = Directory.Exists(items[i]);
                string name = isDirectory ?
                    $"{new DirectoryInfo(items[i]).Name}/" :
                    new FileInfo(items[i]).Name;

                string line = $"{indent}{connector}{name}";
                Console.WriteLine(line);
                outputBuilder.AppendLine(line);

                if (isDirectory)
                {
                    DisplayDirectoryStructure(items[i], newIndent);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            string line = $"{indent}├── (Access Denied)";
            Console.WriteLine(line);
            outputBuilder.AppendLine(line);
        }
        catch (Exception ex)
        {
            string line = $"{indent}├── Error: {ex.Message}";
            Console.WriteLine(line);
            outputBuilder.AppendLine(line);
        }
    }
}