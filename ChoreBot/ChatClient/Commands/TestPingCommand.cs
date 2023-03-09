using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatClient.Commands
{
    internal class TestPingCommand : BaseDiscordCommand
    {
        private const int targetArguementIndex = 0;
        private const int messageArguementIndex = 1;

        public override string Name => "ping";

        public override string Description => "Ping someone";

        public override ApplicationCommandProperties BuildCommand()
        {
            SlashCommandOptionBuilder targetCommandOptionBuilder = new();
            targetCommandOptionBuilder.WithName("target");
            targetCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
            targetCommandOptionBuilder.WithDescription("Target to Ping");
            targetCommandOptionBuilder.WithRequired(true); // Only add this if you want it to be required
            
            SlashCommandOptionBuilder messageOptionBuilder = new();
            messageOptionBuilder.WithName("message");
            messageOptionBuilder.WithType(ApplicationCommandOptionType.String);
            messageOptionBuilder.WithDescription("Message for Target");

            var pingCommandBuilder = new SlashCommandBuilder();
            pingCommandBuilder.WithName(Name);
            pingCommandBuilder.WithDescription(Description);
            pingCommandBuilder.AddOptions(targetCommandOptionBuilder, messageOptionBuilder);

            return pingCommandBuilder.Build();
        }

        protected override async Task RunCommandAsync(SocketSlashCommand command)
        {
            
            var options = command.Data.Options.ToList();
            var userTarget = (string)options[targetArguementIndex].Value ?? string.Empty;

            var users = await command.Channel.GetUsersAsync().FlattenAsync();
            var userWithName = users.SingleOrDefault(user => user.Username == userTarget);
            
            var isUserNameAMention = Mentions.IsMention(userTarget);
            var isActiveUser = userWithName is not null;
            var userFound = isActiveUser || isUserNameAMention;
            if (!userFound)
            {
                await command.RespondAsync($"Could not find user: {userTarget} (Try using an @Mention)");
                return;
            }

            var userNameToMention = string.Empty;
            if (isActiveUser)
            {
                userNameToMention = userWithName?.Mention;
            }
            else if (isUserNameAMention)
            {
                userNameToMention = userTarget;
            }

            //if message present send it as well
            string messageArgument = BuildMessageFromArgument(options);
            var message = string.Join(' ', userNameToMention, messageArgument).Trim();

            await command.RespondAsync(message);

        }

        private string BuildMessageFromArgument(List<SocketSlashCommandDataOption> options)
        {
            string messageArguement = string.Empty;
            try
            {
                if (options.Count > messageArguementIndex)
                {
                    messageArguement = (string)options[messageArguementIndex].Value ?? string.Empty;

                }
            }
            catch
            {
                // message doesnt exist
            }

            return messageArguement;
        }
    }
}
