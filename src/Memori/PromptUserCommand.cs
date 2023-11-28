using Spectre.Console.Cli;

namespace Memori;

public class PromptUserCommand : AsyncCommand<PromptUserSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, PromptUserSettings settings)
    {
        throw new NotImplementedException();
    }
}