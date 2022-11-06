using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot.Commands
{
    internal class TestPingCommand : BaseDiscordCommand
    {
        public override string Name => "ping";

        public override string Description => "Ping someone";

        public override ApplicationCommandProperties BuildCommand()
        {
            SlashCommandOptionBuilder slashCommandOptionBuilder = new();
            slashCommandOptionBuilder.WithName("target");
            slashCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
            slashCommandOptionBuilder.WithDescription("Target to Ping");
            slashCommandOptionBuilder.WithRequired(true); // Only add this if you want it to be required

            var echoCommandBuilder = new SlashCommandBuilder();
            echoCommandBuilder.WithName(Name);
            echoCommandBuilder.WithDescription(Description);
            echoCommandBuilder.AddOptions(slashCommandOptionBuilder);

            return echoCommandBuilder.Build();
        }

        protected override async Task RunCommandAsync(SocketSlashCommand command)
        {
            //await command.RespondAsync($"@{command.Data.Options.First().Value}");
            var userTarget = (string)command.Data.Options.First().Value ?? string.Empty;
            

            var users = await command.Channel.GetUsersAsync().FlattenAsync();
            var userWithName = users.SingleOrDefault(user => user.Username == userTarget);
            if (userWithName is null)
            {
                await command.RespondAsync($"Could not find user with name: {command.Data.Options.First().Value}");
                return;
            }

            await command.RespondAsync(userWithName.Mention);

        }
    }
}
