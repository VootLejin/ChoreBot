using Discord;
using Discord.WebSocket;

namespace ChoreBot.Commands
{
    internal interface IDiscordCommand
    {
        string Name { get; }
        string Description { get; }
        Task HandleSlashCommandAsync(SocketSlashCommand command);
        ApplicationCommandProperties BuildCommand();
    }
}
