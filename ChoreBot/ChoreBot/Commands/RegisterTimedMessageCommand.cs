using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot.Commands
{
    public class RegisterTimedMessageCommand : BaseDiscordCommand
    {
        public RegisterTimedMessageCommand(TimedMessages timedMessages)
        {
            TimedMessagesHolder = timedMessages;
        }

        public override string Name => "register";

        public override string Description => "register channel for a timed message";

        public TimedMessages TimedMessagesHolder { get; }

        public override ApplicationCommandProperties BuildCommand()
        {
            SlashCommandOptionBuilder unRegisterOptionCommandBuilder = new();
            unRegisterOptionCommandBuilder.WithName("unregister");
            unRegisterOptionCommandBuilder.WithType(ApplicationCommandOptionType.Boolean);
            unRegisterOptionCommandBuilder.WithDescription("Unregister command");

            var registerCommandBuilder = new SlashCommandBuilder();
            registerCommandBuilder.WithName(Name);
            registerCommandBuilder.WithDescription(Description);
            registerCommandBuilder.AddOptions(unRegisterOptionCommandBuilder);


            return registerCommandBuilder.Build();

        }

        protected override async Task RunCommandAsync(SocketSlashCommand command)
        {
            if (command.Data.Options.Any())
            {
                TimedMessagesHolder.StopMessages();
                await command.RespondAsync("Unregistered");
                return;
            }
            TimedMessagesHolder.RegisterChannelId(command.ChannelId.Value);
            await command.RespondAsync($"Registered");
        }
    }
}
