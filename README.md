# GameShellLite

A lightweight, simple, and type-safe command parsing and execution library for .NET with valve-like command syntax. Created for building in-game consoles but works for building command-line interfaces, debug consoles, or any application that needs to parse and execute text-based commands with typed arguments.

A command could look like this:
```
setPosition "player1" 100 200 50.5
-- or --
set_position player1 100 200 50.5
```

Targets .Net Standard 2.1. Therefore compatiable with the Unity game engine, as well with .NET Core 3.0+, .NET 5+, .NET Framework 4.7.2+

## High Level API
(Not AOT Compatiable with trimming enabled)

- **Fluent API** - Intuitive method chaining for building commands
- **Type Safety** - Strongly-typed arguments (string, bool, float/double/decimal)
- **Flexible Arguments** - Support for up to 10 arguments with optional parameters
- **Command Aliases** - Multiple names for the same command
- **Case Sensitivity** - Configurable case-sensitive or case-insensitive matching
- **Help System** - Built-in help text support for commands

## Low Level API
(AOT Compatiable)

```csharp
var command = CommandParser.Parse("setPosition ""player1"" 100 200 50.5");

// returns this:

public class CommandParseResult
{
    public string Command { get; } // setPosition
    public IReadOnlyList<object?> Arguments { get; } // contains string player1, then 3 float values
}
```

# High Level API Examples

```csharp
using GameShellLite;

// Create a command runner
var runner = new CommandRunner();

// Register a simple command
runner.RegisterCommand("hello")
      .WithExecution(() => Console.WriteLine("Hello, World!"));

// Execute the command
runner.Execute("hello");
```

## Usage Examples

### Commands with Arguments

```csharp
// Command with a single string argument
runner.RegisterCommand("greet")
      .WithArg<string>()
      .WithExecution(name => Console.WriteLine($"Hello, {name}!"));

runner.Execute("greet \"Alice\"");
// Output: Hello, Alice!
```

### Multiple Arguments with Different Types

```csharp
// Command with multiple typed arguments
runner.RegisterCommand("setPosition")
      .WithArg<float>()  // x coordinate
      .WithArg<float>()  // y coordinate
      .WithArg<float>()  // z coordinate
      .WithExecution((x, y, z) => 
      {
          Console.WriteLine($"Position set to ({x}, {y}, {z})");
      });

runner.Execute("setPosition 10.5 20.0 -5.5");
// Output: Position set to (10.5, 20, -5.5)
```

### Optional Arguments

```csharp
// Command with optional arguments
runner.RegisterCommand("connect")
      .WithArg<string>()           // host (required)
      .WithArg<float>(optional: true)  // port (optional)
      // this would work too, for nullable float
      //.WithArg<float?>(true)
      .WithExecution((host, port) => 
      {
          var portNum = port == 0 ? 8080 : (int)port;
          Console.WriteLine($"Connecting to {host}:{portNum}");
      });

runner.Execute("connect \"localhost\"");
// Output: Connecting to localhost:8080

runner.Execute("connect \"localhost\" 3000");
// Output: Connecting to localhost:3000
```

### Command Aliases

```csharp
// Register a command with aliases
runner.RegisterCommand("help")
      .WithAlias("h")
      .WithAlias("helpme")
      .WithArg<string>()
      .WithExecution(command => Console.WriteLine(runner.GetHelpPrintFor(command)));

runner.Execute("help");   // Works
runner.Execute("h");      // Also works
runner.Execute("helpme"); // Also works
```

### Help Text

```csharp
// Add help documentation to commands
runner.RegisterCommand("spawn")
      .WithHelp("Spawns an entity at the specified coordinates")
      .WithArg<string>()  // entity type
      .WithArg<float>()   // x position
      .WithArg<float>()   // y position
      .WithExecution((type, x, y) => 
      {
          Console.WriteLine($"Spawning {type} at ({x}, {y})");
      });

// Get help text for a command
string? helpText = runner.GetHelpPrintFor("spawn");
Console.WriteLine(helpText);
```

### Boolean Arguments

```csharp
runner.RegisterCommand("setDebug")
      .WithArg<bool>()
      .WithExecution(enabled => 
      {
          Console.WriteLine($"Debug mode: {(enabled ? "ON" : "OFF")}");
      });

runner.Execute("setDebug true");
// Output: Debug mode: ON
```

### Direct Execution with Parse Result

```csharp
// Access the raw parse result for dynamic handling
runner.RegisterCommand("dynamic")
      .WithExecution(result => 
      {
          Console.WriteLine($"Command: {result.Command}");
          Console.WriteLine($"Argument count: {result.Arguments.Count}");
          foreach (var arg in result.Arguments)
          {
              Console.WriteLine($"  - {arg} ({arg?.GetType().Name})");
          }
      });

runner.Execute("dynamic \"test\" 42 true");
// Output:
// Command: dynamic
// Argument count: 3
//   - test (Single)
//   - 42 (Single)
//   - True (Boolean)
```

### Case-Insensitive Commands

```csharp
// Create a case-insensitive command runner
var runner = new CommandRunner(caseInsensitiveCommandNames: true);

runner.RegisterCommand("Test")
      .WithExecution(() => Console.WriteLine("Test executed"));

runner.Execute("test");   // Works
runner.Execute("TEST");   // Also works
runner.Execute("TeSt");   // Also works
```

## Command String Syntax

### String Arguments
Strings must be enclosed in double quotes:
```
greet "John Doe"
```

### Escape Sequences
Supported escape sequences in strings:
- `\\` - Backslash
- `\n` - Newline

```csharp
runner.Execute("print \"Line 1\\nLine 2\"");
runner.Execute("setPath \"C:\\\\Users\\\\Player\"");
```

### Number Arguments
Numbers can be integers or decimals, with optional negative sign:
```
setHealth 100
setSpeed 12.5
adjust -10.5
```

### Boolean Arguments
Use `true` or `false` (case-insensitive):
```
setDebug true
enableCheats false
```

### Identifier Arguments
Unquoted alphanumeric strings:
```
enable debugMode
set config production
```

### Null Arguments
Use `null` keyword (if enabled in parser options, enabled by default):
```
setName null
```

Control whether null values are allowed:

```csharp
var options = new CommandParserOptions
{
    AllowNull = false  // Disallow null arguments with null literal
};

var runner = new CommandRunner(parserOptions: options);
```

### Number Precision

Choose the numeric type for number arguments (default is float):

```csharp
var options = new CommandParserOptions
{
    NumberPrecision = NumberPrecision.Double  // Float, Double, or Decimal
};

var runner = new CommandRunner(parserOptions: options);

runner.RegisterCommand("calculate")
      .WithArg<double>()  // Must match the NumberPrecision setting
      .WithExecution(value => Console.WriteLine($"Value: {value}"));
```

### Exceptions

- `CommandRunnerException` - General command runner errors
- `CommandRunnerNotFoundException` - Command not found
- `CommandParserException` - Parsing errors

### Error Handling

```csharp
try
{
    runner.Execute(userInput);
}
catch (CommandRunnerNotFoundException ex)
{
    Console.WriteLine($"Command not found: {ex.CommandName}");
}
catch (CommandRunnerException ex)
{
    Console.WriteLine($"Command error: {ex.Message}");
}
catch (CommandParserException ex)
{
    Console.WriteLine($"Parse error: {ex.Message}");
}
```
