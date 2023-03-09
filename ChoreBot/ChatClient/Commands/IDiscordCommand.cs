using Discord;
using Discord.WebSocket;

namespace ChatClient.Commands
{
    public interface IDiscordCommand
    {
        string Name { get; }
        string Description { get; }
        Task HandleSlashCommandAsync(SocketSlashCommand command);
        ApplicationCommandProperties BuildCommand();
    }
}
