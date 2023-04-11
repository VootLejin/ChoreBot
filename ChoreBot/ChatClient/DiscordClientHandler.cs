using Core.Interfaces;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace ChatClient
{
    public class DiscordClientHandler
    {
        private readonly IChoreService _choreService;
        private readonly DiscordSocketClient _client;

        public DiscordClientHandler(DiscordSocketClient client, IChoreService choreService)
        {
            _client = client;
            _choreService = choreService;

            _choreService.ChoreEnded += async (sender, userAndChannelId) => await OnChoreEnded(sender, userAndChannelId);

        }

        private async Task OnChoreEnded(object sender, ChoreEndDetails endDetails)
        {
            var user = endDetails.User;
            var channelId = endDetails.ChannelId;

            var channel = _client.GetChannel(channelId) as IMessageChannel;
            if (channel != null)
            {
                await channel.SendMessageAsync($"{user} has completed their chore.");
            }
            else
            {
                throw new InvalidOperationException("Could not find the specified channel.");
            }
        }
    }
}
