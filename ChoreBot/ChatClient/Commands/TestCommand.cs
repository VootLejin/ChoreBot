using Discord;
using Discord.WebSocket;

namespace ChatClient.Commands
{
    public class TestCommand : BaseDiscordCommand
    {
        public override string Name => "test";

        public override string Description => "A test command to see what happens";

        public override ApplicationCommandProperties BuildCommand()
        {
            // Will move to base class
            var testCommand = new SlashCommandBuilder();
            testCommand.WithName(Name);
            testCommand.WithDescription(Description);
            //testCommand.WithDefaultPermission(true);
            return testCommand.Build();
        }

        protected override async Task RunCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync($"You executed {command.Data.Name}");
        }
    }
}
