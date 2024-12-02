using System;
using System.IO;
using System.Linq;
using System.Text;

class DirectoryTree
{
    private static StringBuilder outputBuilder = new StringBuilder();

    static void Main(string[] args)
    {
        // No given params
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a directory path as an argument.");
            Console.WriteLine("Usage: dotnet run -- <directory_path> [output_filename]");
            return;
        }

        string rootPath = args[0];
        string outputFile = args.Length > 1 ? args[1] : "directory-tree.txt";

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
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            var items = directories.Concat(files).ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                bool isLast = (i == items.Length - 1);
                string connector = isLast ? "└── " : "├── ";
                string newIndent = indent + (isLast ? "    " : "│   ");

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