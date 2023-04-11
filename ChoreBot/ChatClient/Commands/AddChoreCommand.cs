using Core.Interfaces;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatClient.Commands
{
    internal class AddChoreCommand : BaseDiscordCommand
    {
        private const int targetArguementIndex = 0;
        private const int dueTimeArgumentIndex = 1;
        private const int messageArguementIndex = 2;
        private readonly IChoreService _choreService;

        public AddChoreCommand(IChoreService choreService)
        {
            _choreService = choreService;
        }

        public override string Name => "add-chore";

        public override string Description => "assign a chore to someone";

        public override ApplicationCommandProperties BuildCommand()
        {
            SlashCommandOptionBuilder targetCommandOptionBuilder = new();
            targetCommandOptionBuilder.WithName("target");
            targetCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
            targetCommandOptionBuilder.WithDescription("Who to assign the chore to");
            targetCommandOptionBuilder.WithRequired(true); // Only add this if you want it to be required

            SlashCommandOptionBuilder messageOptionBuilder = new();
            messageOptionBuilder.WithName("description");
            messageOptionBuilder.WithType(ApplicationCommandOptionType.String);
            messageOptionBuilder.WithDescription("description of the chore");

            var addChoreCommandBuilder = new SlashCommandBuilder();
            addChoreCommandBuilder.WithName(Name);
            addChoreCommandBuilder.WithDescription(Description);
            addChoreCommandBuilder.AddOptions(targetCommandOptionBuilder, messageOptionBuilder);

            SlashCommandOptionBuilder dueTimeOptionBuilder = new();
            dueTimeOptionBuilder.WithName("due_time");
            dueTimeOptionBuilder.WithType(ApplicationCommandOptionType.String);
            dueTimeOptionBuilder.WithDescription("The due time for the chore. Format DD-MM-YYYY HH:mm");
            dueTimeOptionBuilder.WithRequired(true); // Only add this if you want it to be required
            addChoreCommandBuilder.AddOption(dueTimeOptionBuilder);


            return addChoreCommandBuilder.Build();
        }

        protected override async Task RunCommandAsync(SocketSlashCommand command)
        {
            var options = command.Data.Options.ToList();
            var userTarget = (string)options[targetArguementIndex].Value ?? string.Empty;

            var discordUser = await FindDiscordUserAsync(userTarget, command);

            if (discordUser is null)
            {
                await command.RespondAsync($"Could not find user: {userTarget} (Try using an @Mention)");
                return;
            }

            var userNameToMention = discordUser.Mention;

            //if message present send it as well
            string messageArgument = ExtractChoreDescription(options);
            DateTimeOffset dueTime = ExtractChoreDueTime(options);
            if(dueTime < DateTimeOffset.Now)
            {
                await command.RespondAsync($"Due time must be in the future.");
                return;
            }

            await _choreService.AddChoreAsync(userNameToMention, messageArgument, command.ChannelId.Value, dueTime);

            await command.RespondAsync($"chore added for {discordUser.Username}");
        }

        private DateTimeOffset ExtractChoreDueTime(List<SocketSlashCommandDataOption> options)
        {

            string dueTimeString = (string)options[dueTimeArgumentIndex].Value ?? string.Empty;

            if (DateTimeOffset.TryParse(dueTimeString, out DateTimeOffset dueTime))
            {
                return dueTime;
            }
            else
            {
                throw new ArgumentException("Invalid due time format.");
            }
        }

        private async Task<IUser?> FindDiscordUserAsync(string userTarget, SocketSlashCommand command)
        {
            var isUserNameAMention = Mentions.IsMention(userTarget);
            var users = await command.Channel.GetUsersAsync().FlattenAsync();

            if (isUserNameAMention)
            {
                var userId = userTarget.Replace(@"<@", string.Empty).Replace(@">", string.Empty);
                var userIdAsUlong = ulong.Parse(userId);
                return users.SingleOrDefault(user => user.Id == userIdAsUlong);
            }
            return users.SingleOrDefault(user => user.Username == userTarget);

        }

        private string ExtractChoreDescription(List<SocketSlashCommandDataOption> options)
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
