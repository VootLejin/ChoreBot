﻿using ChatClient.Commands;
using Core.Interfaces;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;
using System.Windows.Input;

namespace ChatClient
{
    public class DiscordChatClient : IChatClient
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        //private TimedMessages _timedMessages;
        private IServiceProvider _services;
        private bool _isConnected = false;
        private List<ApplicationCommandProperties> ApplicationCommandProperties = new List<ApplicationCommandProperties>();

        public DiscordChatClient()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                // How much logging do you want to see?
                LogLevel = LogSeverity.Info,

                // If you or another service needs to do anything with messages
                // (eg. checking Reactions, checking the content of edited/deleted messages),
                // you must set the MessageCacheSize. You may adjust the number as needed.
                //MessageCacheSize = 50,

                // If your platform doesn't have native WebSockets,
                // add Discord.Net.Providers.WS4Net from NuGet,
                // add the `using` at the top, and uncomment this line:
                //WebSocketProvider = WS4NetProvider.Instance
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                // Again, log level:
                LogLevel = LogSeverity.Info,

                // There's a few more properties you can set,
                // for example, case-insensitive commands.
                CaseSensitiveCommands = false,
            });

            // Subscribe the logging handler to both the client and the CommandService.
            _client.Log += Log;
            _commands.Log += Log;
            //_timedMessages = new TimedMessages(_client);
        }

        public async Task InitializeAsync(IServiceProvider services)
        {
            _services = services;

            // Centralize the logic for commands into a separate method.
            await InitCommands();

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot,
                GetBotToken());


            await _client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            //await Task.Delay(Timeout.Infinite);
            await TaskEx.WaitUntil(() => _isConnected);
        }

        private async Task InitCommands()
        {
            // Either search the program and add all Module classes that can be found.
            // Module classes MUST be marked 'public' or they will be ignored.
            // You also need to pass your 'IServiceProvider' instance now,
            // so make sure that's done before you get here.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Or add Modules manually if you prefer to be a little more explicit:
            // await _commands.AddModuleAsync<SomeModule>(_services);
            // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

            // Subscribe a handler to see if a message invokes a command.
            //_client.MessageReceived += HandleCommandAsync;
            _client.Connected += Client_Ready;
            //_client.SlashCommandExecuted+= HandleCommandAsync;
        }

        private static string GetBotToken()
        {
            // @Dave, Hit me up to get the token.
            var token = File.ReadAllText("token.txt");
            return token;
            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;
        }

        private async Task Client_Ready()
        {
            var commands = _services.GetServices<IDiscordCommand>();

            await ReadyCommands(commands);
            AttachCommandListeners(commands);
            _isConnected = true;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadyCommands(IEnumerable<IDiscordCommand> commands)
        {
            ApplicationCommandProperties = new List<ApplicationCommandProperties>(commands.Select(c => c.BuildCommand()).ToList());
            try
            {
                await _client.BulkOverwriteGlobalApplicationCommandsAsync(ApplicationCommandProperties.ToArray());
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException.
                // This exception contains the path of the error as well as the error message.
                // You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        private void AttachCommandListeners(IEnumerable<IDiscordCommand> commands)
        {
            foreach (var command in commands)
            {
                _client.SlashCommandExecuted += command.HandleSlashCommandAsync;
            }
        }

        public async Task SendMessageAsync(string assignee, string description, ulong channelId)
        {
            var channel = await _client.GetChannelAsync(channelId) as IMessageChannel;
            await channel.SendMessageAsync($"{assignee} {description}");
        }
    }
}