﻿using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;
using System.Windows.Input;

namespace ChoreBot
{
    public class Program
    {
        private readonly DiscordSocketClient _client;

        // Keep the CommandService and DI container around for use with commands.
        // These two types require you install the Discord.Net.Commands package.
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public Program()
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

            // Setup your DI container.
            _services = ConfigureServices();
        }

        public static Task Main(string[] args)
        {
            // Call the Program constructor, followed by the 
            // MainAsync method and wait until it finishes (which should be never).
            return new Program().MainAsync();
        }

        public async Task MainAsync()
        {

            // Centralize the logic for commands into a separate method.
            await InitCommands();

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot,
                GetBotToken());
            await _client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
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
            _client.SlashCommandExecuted+= HandleCommandAsync;
        }

        public async Task Client_Ready()
        {
            var testCommand = new SlashCommandBuilder();
            testCommand.WithName("test");
            testCommand.WithDescription("A test command to see what happens");
            testCommand.WithDefaultPermission(true);

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(testCommand.Build());
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }

        }

        // If any services require the client, or the CommandService, or something else you keep on hand,
        // pass them as parameters into this method as needed.
        // If this method is getting pretty long, you can seperate it out into another file using partials.
        private static IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection()
                // Repeat this for all the service classes
                // and other dependencies that your commands might need.
                .AddSingleton<CommandHandler>();

            // When all your required services are in the collection, build the container.
            // Tip: There's an overload taking in a 'validateScopes' bool to make sure
            // you haven't made any mistakes in your dependency graph.
            return map.BuildServiceProvider();
        }

        private async Task HandleCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync($"You executed {command.Data.Name}");
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

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}