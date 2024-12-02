# Directory Tree

## Usage

```bash
# Basic usage
dotnet run -- .

# With output file
dotnet run -- . output.txt

# With exclusions
dotnet run -- . -e bin obj

# With output file and exclusions
dotnet run -- . output.txt -e bin obj node_modules

```
