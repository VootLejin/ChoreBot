using Discord;
using Discord.WebSocket;

namespace ChoreBot.Commands
{
    public abstract class BaseDiscordCommand : IDiscordCommand
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract ApplicationCommandProperties BuildCommand();
        public async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            if (command.Data.Name != Name)
            {
                return;
            }
            await RunCommandAsync(command);
        }

        protected abstract Task RunCommandAsync(SocketSlashCommand command);
    }
}
