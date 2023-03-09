using ChatClient;
using Core.Interfaces;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;

namespace ChoreBot
{
    public class Program
    {
        private readonly IServiceProvider _services;
        private readonly DiscordChatClient _chatClient;
        //private TimedMessages _timedMessages;

        public Program()
        {
            // Setup your DI container.
            //_timedMessages = new TimedMessages(_client);
            var serviceCollection = ServiceCollector.ConfigureChatClientServices();
            ConfigureChoreBotServices(serviceCollection);
            _services = serviceCollection.BuildServiceProvider();
            _chatClient = new DiscordChatClient(_services);
        }

        private void ConfigureChoreBotServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IChoreService, ChoreService>();
        }

        public static Task Main(string[] args)
        {
            // Call the Program constructor, followed by the 
            // MainAsync method and wait until it finishes (which should be never).
            return new Program().MainAsync();
        }

        public async Task MainAsync()
        {
            await _chatClient.InitializeAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }
    }
}