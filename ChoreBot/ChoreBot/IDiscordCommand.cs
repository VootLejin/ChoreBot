using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot
{
    internal interface IDiscordCommand
    {
        string Name { get; }
        string Description { get; }
        Task HandleSlashCommandAsync(SocketSlashCommand command);
        ApplicationCommandProperties BuildCommand();
    }
}
