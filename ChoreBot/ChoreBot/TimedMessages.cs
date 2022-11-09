using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot
{
    public class TimedMessages
    {
        public TimedMessages(IDiscordClient client)
        {
            Client = client;

            Timer = new Timer(OnTimerTick);
        }

        private async void OnTimerTick(object? state)
        {
            var channel = await Client.GetChannelAsync(ChannelId) as IMessageChannel;
            await channel.SendMessageAsync("Beep Beep!");
        }

        public IDiscordClient Client { get; }

        private Timer Timer { get; }
        public ulong ChannelId { get; private set; }

        public void RegisterChannelId(ulong channelId)
        {
            // TODO: Verification logic
            ChannelId = channelId;
            Timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5.0));
        }

        public void StopMessages()
        {
            // SHADDAP!
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
