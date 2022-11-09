using Discord;
using Discord.WebSocket;

namespace ChoreBot.Commands
{
    public class EchoCommand : BaseDiscordCommand
    {
        public override string Name => "echo";

        public override string Description => "echo the given text";

        public override ApplicationCommandProperties BuildCommand()
        {
            SlashCommandOptionBuilder slashCommandOptionBuilder = new();
            slashCommandOptionBuilder.WithName("text");
            slashCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
            slashCommandOptionBuilder.WithDescription(Description);
            slashCommandOptionBuilder.WithRequired(true); // Only add this if you want it to be required

            var echoCommandBuilder = new SlashCommandBuilder();
            echoCommandBuilder.WithName(Name);
            echoCommandBuilder.WithDescription(Description);
            echoCommandBuilder.AddOptions(slashCommandOptionBuilder);

            return echoCommandBuilder.Build();
        }

        protected override async Task RunCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync($"{command.Data.Options.First().Value}");
        }
    }
}
