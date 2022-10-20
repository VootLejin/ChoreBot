using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot
{
    public class EchoCommand : IDiscordCommand
    {
        public string Name => "echo";

        public string Description => "echo the given text";

        public ApplicationCommandProperties BuildCommand()
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

        public async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            if (command.Data.Name != Name)
            {
                return;
            }
            await command.RespondAsync($"{command.Data.Options.First().Value}");
        }
    }
}
