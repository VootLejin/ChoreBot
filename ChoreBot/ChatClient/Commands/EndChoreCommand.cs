using Core.Interfaces;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatClient.Commands
{
    internal class EndChoreCommand : BaseDiscordCommand
    {
        private const int targetArguementIndex = 0;
        private readonly IChoreService _choreService;

        public EndChoreCommand(IChoreService choreService)
        {
            _choreService = choreService;
        }

        public override string Name => "end-chore";

        public override string Description => "Finish a chore";

        public override ApplicationCommandProperties BuildCommand()
        {
            SlashCommandOptionBuilder targetCommandOptionBuilder = new();
            targetCommandOptionBuilder.WithName("target");
            targetCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
            targetCommandOptionBuilder.WithDescription("Who finished the chore");
            targetCommandOptionBuilder.WithRequired(true); // Only add this if you want it to be required

            var endChoreCommandBuilder = new SlashCommandBuilder();
            endChoreCommandBuilder.WithName(Name);
            endChoreCommandBuilder.WithDescription(Description);
            endChoreCommandBuilder.AddOptions(targetCommandOptionBuilder);

            return endChoreCommandBuilder.Build();
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

            await _choreService.EndChoreAsync(userNameToMention, command.ChannelId.Value);

            await command.RespondAsync($"Chore completed for {discordUser.Username}");
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
    }
}
