using Discord;
using Discord.WebSocket;

namespace ChoreBot
{
    public class TestCommand : IDiscordCommand
    {
        public string Name => "test";

        public string Description => "A test command to see what happens";

        public ApplicationCommandProperties BuildCommand()
        {
            // Will move to base class
            var testCommand = new SlashCommandBuilder();
            testCommand.WithName(Name);
            testCommand.WithDescription(Description);
            //testCommand.WithDefaultPermission(true);
            return testCommand.Build();
        }

        public async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            if (command.Data.Name != Name)
            {
                return;
            }
            await command.RespondAsync($"You executed {command.Data.Name}");
        }
    }
}
