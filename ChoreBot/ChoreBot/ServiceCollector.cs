﻿using ChoreBot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ChoreBot
{
    public static class ServiceCollector
    {

        // If any services require the client, or the CommandService, or something else you keep on hand,
        // pass them as parameters into this method as needed.
        // If this method is getting pretty long, you can seperate it out into another file using partials.
        public static IServiceProvider ConfigureServices(TimedMessages timedMessages)
        {
            var map = new ServiceCollection()
                .AddSingleton(timedMessages)
                // Repeat this for all the service classes
                // and other dependencies that your commands might need.
                .AddSingleton<IDiscordCommand, TestCommand>()
                .AddSingleton<IDiscordCommand, TestPingCommand>()
                .AddSingleton<IDiscordCommand, RegisterTimedMessageCommand>()
                .AddSingleton<IDiscordCommand, EchoCommand>();

            // When all your required services are in the collection, build the container.
            // Tip: There's an overload taking in a 'validateScopes' bool to make sure
            // you haven't made any mistakes in your dependency graph.
            return map.BuildServiceProvider();
        }
    }
}