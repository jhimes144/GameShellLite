namespace GameShellLite.Tests;

public class CommandParsingTests
{
    [Fact]
    public void Basic_Only_Command()
    {
        var command = CommandParser.Parse("enableCheats");
        Assert.Equivalent("enableCheats", command.Command);
        Assert.Empty(command.Arguments);
    }

    [Fact]
    public void Command_String_Arg()
    {
        var command = CommandParser.Parse("enableCheats \"hello world\"");
        Assert.Equivalent("enableCheats", command.Command);
        Assert.Equivalent("hello world", command.Arguments[0]);
    }

    [Fact]
    public void Command_Multiple_String_Args()
    {
        var command = CommandParser.Parse("setName \"John\" \"Doe\"");
        Assert.Equivalent("setName", command.Command);
        Assert.Equal(2, command.Arguments.Count);
        Assert.Equivalent("John", command.Arguments[0]);
        Assert.Equivalent("Doe", command.Arguments[1]);
    }

    [Fact]
    public void Command_With_Number_Arg()
    {
        var command = CommandParser.Parse("setHealth 100");
        Assert.Equivalent("setHealth", command.Command);
        Assert.Single(command.Arguments);
        Assert.Equal(100f, command.Arguments[0]);
    }

    [Fact]
    public void Command_With_Decimal_Number()
    {
        var command = CommandParser.Parse("setSpeed 12.5");
        Assert.Equivalent("setSpeed", command.Command);
        Assert.Equal(12.5f, command.Arguments[0]);
    }

    [Fact]
    public void Command_With_Identifier_Arg()
    {
        var command = CommandParser.Parse("enable debugMode");
        Assert.Equivalent("enable", command.Command);
        Assert.Equivalent("debugMode", command.Arguments[0]);
    }

    [Fact]
    public void Command_With_Mixed_Args()
    {
        var command = CommandParser.Parse("spawn enemy 5 \"Boss Monster\"");
        Assert.Equivalent("spawn", command.Command);
        Assert.Equal(3, command.Arguments.Count);
        Assert.Equivalent("enemy", command.Arguments[0]);
        Assert.Equal(5f, command.Arguments[1]);
        Assert.Equivalent("Boss Monster", command.Arguments[2]);
    }

    [Fact]
    public void String_With_Escaped_Backslash()
    {
        var command = CommandParser.Parse("setPath \"C:\\\\Users\\\\Player\"");
        Assert.Equivalent("setPath", command.Command);
        Assert.Equivalent("C:\\Users\\Player", command.Arguments[0]);
    }

    [Fact]
    public void String_With_Newline_Escape()
    {
        var command = CommandParser.Parse("print \"Line1\\nLine2\"");
        Assert.Equivalent("print", command.Command);
        Assert.Equivalent("Line1\nLine2", command.Arguments[0]);
    }

    [Fact]
    public void String_With_Multiple_Escapes()
    {
        var command = CommandParser.Parse("format \"Path: C:\\\\Data\\nStatus: OK\"");
        Assert.Equivalent("format", command.Command);
        Assert.Equivalent("Path: C:\\Data\nStatus: OK", command.Arguments[0]);
    }

    [Fact]
    public void Empty_String_Arg()
    {
        var command = CommandParser.Parse("setName \"\"");
        Assert.Equivalent("setName", command.Command);
        Assert.Equivalent("", command.Arguments[0]);
    }

    [Fact]
    public void Command_With_Negative_Number()
    {
        var command = CommandParser.Parse("adjust -10.5");
        Assert.Equivalent("adjust", command.Command);
        Assert.Equal(-10.5f, command.Arguments[0]);
    }

    [Fact]
    public void Command_With_Multiple_Numbers()
    {
        var command = CommandParser.Parse("setPosition 100 200 50.5");
        Assert.Equivalent("setPosition", command.Command);
        Assert.Equal(3, command.Arguments.Count);
        Assert.Equal(100f, command.Arguments[0]);
        Assert.Equal(200f, command.Arguments[1]);
        Assert.Equal(50.5f, command.Arguments[2]);
    }

    [Fact]
    public void Command_With_Zero()
    {
        var command = CommandParser.Parse("reset 0");
        Assert.Equivalent("reset", command.Command);
        Assert.Equal(0f, command.Arguments[0]);
    }

    [Fact]
    public void String_With_Only_Spaces()
    {
        var command = CommandParser.Parse("test \"   \"");
        Assert.Equivalent("test", command.Command);
        Assert.Equivalent("   ", command.Arguments[0]);
    }

    [Fact]
    public void Complex_Command_With_All_Types()
    {
        var command = CommandParser.Parse("configure server 8080 \"localhost\" true 3.14");
        Assert.Equal("configure", command.Command);
        Assert.Equal(5, command.Arguments.Count);
        Assert.Equal("server", command.Arguments[0]);
        Assert.Equal(8080f, command.Arguments[1]);
        Assert.Equal("localhost", command.Arguments[2]);
        Assert.Equal(true, command.Arguments[3]);
        Assert.Equal(3.14f, command.Arguments[4]);
    }

    [Fact]
    public void String_With_Escaped_Backslash_At_End()
    {
        var command = CommandParser.Parse("test \"path\\\\\"");
        Assert.Equivalent("test", command.Command);
        Assert.Equivalent("path\\", command.Arguments[0]);
    }

    [Fact]
    public void Multiple_Identifiers()
    {
        var command = CommandParser.Parse("set config debug enabled");
        Assert.Equivalent("set", command.Command);
        Assert.Equal(3, command.Arguments.Count);
        Assert.Equivalent("config", command.Arguments[0]);
        Assert.Equivalent("debug", command.Arguments[1]);
        Assert.Equivalent("enabled", command.Arguments[2]);
    }

    [Fact]
    public void Invalid_Escape_Sequence_Throws()
    {
        Assert.Throws<Exception>(() => CommandParser.Parse("test \"hello\\t\""));
    }

    [Fact]
    public void Unterminated_String_Throws()
    {
        Assert.Throws<Exception>(() => CommandParser.Parse("test \"hello"));
    }

    [Fact]
    public void Empty_Command_String_Throws()
    {
        Assert.Throws<Exception>(() => CommandParser.Parse(""));
    }

    [Fact]
    public void Only_Whitespace_Throws()
    {
        Assert.Throws<Exception>(() => CommandParser.Parse("   "));
    }
}