namespace GameShellLite.Tests;

public class CommandRunnerTests
    {
        [Fact]
        public void Basic_Direct()
        {
            string? commandCalled = null;

            var runner = new CommandRunner();
            runner.RegisterCommand("test")
                  .WithExecution(c =>
                  {
                      commandCalled = c.Command;
                  });

            runner.Execute("test");
            Assert.Equal("test", commandCalled);
        }

        [Fact]
        public void Execute_WithNoArguments_CallsAction()
        {
            bool wasCalled = false;

            var runner = new CommandRunner();
            runner.RegisterCommand("hello")
                  .WithExecution(() =>
                  {
                      wasCalled = true;
                  });

            runner.Execute("hello");
            
            Assert.True(wasCalled);
        }

        [Fact]
        public void Execute_WithOneArgument_PassesCorrectValue()
        {
            string? receivedArg = null;

            var runner = new CommandRunner();
            runner.RegisterCommand("greet")
                  .WithArg<string>()
                  .WithExecution(name =>
                  {
                      receivedArg = name;
                  });

            runner.Execute("greet \"John\"");
            
            Assert.Equal("John", receivedArg);
        }

        [Fact]
        public void Execute_WithTwoArguments_PassesCorrectValues()
        {
            string? receivedName = null;
            float receivedAge = 0;

            var runner = new CommandRunner();
            runner.RegisterCommand("person")
                  .WithArg<string>()
                  .WithArg<float>()
                  .WithExecution((name, age) =>
                  {
                      receivedName = name;
                      receivedAge = age;
                  });

            runner.Execute("person \"Alice\" 25");
            
            Assert.Equal("Alice", receivedName);
            Assert.Equal(25f, receivedAge);
        }

        [Fact]
        public void Execute_WithAlias_ExecutesCommand()
        {
            bool wasCalled = false;

            var runner = new CommandRunner();
            runner.RegisterCommand("help")
                  .WithAlias("h")
                  .WithAlias("k")
                  .WithExecution(() =>
                  {
                      wasCalled = true;
                  });

            runner.Execute("h");
            
            Assert.True(wasCalled);
        }

        [Fact]
        public void Execute_CaseInsensitive_MatchesCommand()
        {
            bool wasCalled = false;

            var runner = new CommandRunner(caseInsensitiveCommandNames: true);
            runner.RegisterCommand("Test")
                  .WithExecution(() =>
                  {
                      wasCalled = true;
                  });

            runner.Execute("test");
            
            Assert.True(wasCalled);
        }

        [Fact]
        public void Execute_CaseSensitive_DoesNotMatchDifferentCase()
        {
            bool wasCalled = false;

            var runner = new CommandRunner(caseInsensitiveCommandNames: false);
            runner.RegisterCommand("Test")
                  .WithExecution(() =>
                  {
                      wasCalled = true;
                  });

            var result = runner.GetIsValidCommand("test");
            
            Assert.False(result);
            Assert.False(wasCalled);
        }

        [Fact]
        public void Execute_UnknownCommand_ThrowsException()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("known")
                  .WithExecution(() => { });

            Assert.Throws<CommandRunnerNotFoundException>(() => runner.Execute("unknown"));
        }

        [Fact]
        public void Execute_CommandWithoutExecution_ThrowsException()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("incomplete");

            Assert.Throws<CommandRunnerException>(() => runner.Execute("incomplete"));
        }

        [Fact]
        public void Execute_TooManyArguments_ThrowsException()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("single")
                  .WithArg<string>()
                  .WithExecution(arg => { });

            Assert.Throws<CommandRunnerException>(() => runner.Execute("single \"one\" \"two\""));
        }

        [Fact]
        public void Execute_NonOptional_After_Optional_ThrowsException()
        {
            var runner = new CommandRunner();

            Assert.Throws<CommandRunnerException>(() => runner.RegisterCommand("single")
                  .WithArg<string>(true)
                  .WithArg<double>()
                  .WithExecution((arg, arg2) => { }));
        }

        [Fact]
        public void Execute_WithNullArgument_PassesNull()
        {
            string? receivedArg = "initial";

            var runner = new CommandRunner();
            runner.RegisterCommand("nullable")
                  .WithArg<string>()
                  .WithExecution(arg =>
                  {
                      receivedArg = arg;
                  });

            runner.Execute("nullable null");
            
            Assert.Null(receivedArg);
        }

        [Fact]
        public void Execute_WithFewerArgumentsThanExpected_Optional_UsesDefaults()
        {
            string? receivedName = "initial";
            float receivedAge = -1;

            var runner = new CommandRunner();
            runner.RegisterCommand("partial")
                  .WithArg<string>(true)
                  .WithArg<float>(true)
                  .WithExecution((name, age) =>
                  {
                      receivedName = name;
                      receivedAge = age;
                  });

            runner.Execute("partial");
            
            Assert.Null(receivedName);
            Assert.Equal(0, receivedAge);
        }

    [Fact]
    public void Execute_Optional_Nullable_Specified()
    {
        float receivedAge = -1;

        var runner = new CommandRunner();
        runner.RegisterCommand("partial_with_null")
              .WithArg<float?>(true)
              .WithExecution(age =>
              {
                  receivedAge = age!.Value;
              });

        runner.Execute("partial_with_null 12");
        Assert.Equal(12f, receivedAge);
    }

    [Fact]
        public void Execute_Optional_UsesDefaults_Nullables()
        {
            float? receivedAge = -1;

            var runner = new CommandRunner();
            runner.RegisterCommand("partial")
                  .WithArg<float?>(true)
                  .WithExecution(age =>
                  {
                      receivedAge = age;
                  });

            runner.Execute("partial");
        Assert.Null(receivedAge);
    }

        [Fact]
        public void Help_Text()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("test")
                  .WithHelp("This command does stuff")
                  .WithArg<string>(false)
                  .WithExecution(testStr =>
                  {
                  });

            var helpText = runner.GetHelpPrintFor("test");
            Assert.NotEmpty(helpText!);
        }

        [Fact]
        public void Execute_Wrong_Arg_Type()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("test")
                  .WithArg<string>(false)
                  .WithExecution(testStr =>
                  {
                  });

            Assert.Throws<CommandRunnerException>(() => runner.Execute("test 12"));
        }

        [Fact]
        public void RemoveCommand_ExistingCommand_ReturnsTrue()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("temp")
                  .WithExecution(() => { });

            var result = runner.RemoveCommand("temp");
            
            Assert.True(result);
            Assert.False(runner.GetIsValidCommand("temp"));
        }

        [Fact]
        public void RemoveCommand_NonExistingCommand_ReturnsFalse()
        {
            var runner = new CommandRunner();

            var result = runner.RemoveCommand("nonexistent");
            
            Assert.False(result);
        }

        [Fact]
        public void ClearCommands_RemovesAllCommands()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("cmd1").WithExecution(() => { });
            runner.RegisterCommand("cmd2").WithExecution(() => { });
            runner.RegisterCommand("cmd3").WithExecution(() => { });

            runner.ClearCommands();
            
            Assert.False(runner.GetIsValidCommand("cmd1"));
            Assert.False(runner.GetIsValidCommand("cmd2"));
            Assert.False(runner.GetIsValidCommand("cmd3"));
        }

        [Fact]
        public void GetIsValidCommand_ExistingCommand_ReturnsTrue()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("valid")
                  .WithExecution(() => { });

            var result = runner.GetIsValidCommand("valid");
            
            Assert.True(result);
        }

        [Fact]
        public void GetIsValidCommand_NonExistingCommand_ReturnsFalse()
        {
            var runner = new CommandRunner();

            var result = runner.GetIsValidCommand("invalid");
            
            Assert.False(result);
        }

        [Fact]
        public void GetIsValidCommand_WithAlias_ReturnsTrue()
        {
            var runner = new CommandRunner();
            runner.RegisterCommand("command")
                  .WithAlias("cmd")
                  .WithExecution(() => { });

            var result = runner.GetIsValidCommand("cmd");
            
            Assert.True(result);
        }

        [Fact]
        public void Execute_WithBooleanArgument_ParsesCorrectly()
        {
            bool receivedValue = false;

            var runner = new CommandRunner();
            runner.RegisterCommand("toggle")
                  .WithArg<bool>()
                  .WithExecution(value =>
                  {
                      receivedValue = value;
                  });

            runner.Execute("toggle true");
            
            Assert.True(receivedValue);
        }

        [Fact]
        public void Execute_WithDoubleArgument_ParsesCorrectly()
        {
            float receivedValue = 0.0f;

            var runner = new CommandRunner();
            runner.RegisterCommand("calculate")
                  .WithArg<float>()
                  .WithExecution(value =>
                  {
                      receivedValue = value;
                  });

            runner.Execute("calculate 3.14");
            
            Assert.Equal(3.14f, receivedValue);
        }

        [Fact]
        public void RegisterCommand_MultipleCommands_AllExecuteCorrectly()
        {
            int cmd1Count = 0;
            int cmd2Count = 0;

            var runner = new CommandRunner();
            runner.RegisterCommand("cmd1").WithExecution(() => cmd1Count++);
            runner.RegisterCommand("cmd2").WithExecution(() => cmd2Count++);

            runner.Execute("cmd1");
            runner.Execute("cmd2");
            runner.Execute("cmd1");
            
            Assert.Equal(2, cmd1Count);
            Assert.Equal(1, cmd2Count);
        }

        [Fact]
        public void Execute_DirectExecution_ReceivesParseResult()
        {
            CommandParseResult? receivedResult = null;

            var runner = new CommandRunner();
            runner.RegisterCommand("inspect")
                  .WithExecution(result =>
                  {
                      receivedResult = result;
                  });

            runner.Execute("inspect");
            
            Assert.NotNull(receivedResult);
            Assert.Equal("inspect", receivedResult.Command);
        }

        [Fact]
        public void Execute_DirectExecutionWithArguments_ReceivesAllArguments()
        {
            CommandParseResult? receivedResult = null;

            var runner = new CommandRunner();
            runner.RegisterCommand("multi")
                  .WithExecution(result =>
                  {
                      receivedResult = result;
                  });

            runner.Execute("multi \"arg1\" 42 true");
            
            Assert.NotNull(receivedResult);
            Assert.Equal(3, receivedResult.Arguments.Count);
            Assert.Equal("arg1", receivedResult.Arguments[0]);
            Assert.Equal(42f, receivedResult.Arguments[1]);
            Assert.Equal(true, receivedResult.Arguments[2]);
        }
    }
