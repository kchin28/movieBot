using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using dbot.Services;

namespace dbot
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            var discordToken = TokenManager.getToken(TokenKey.DiscordToken);
            var omdbToken = TokenManager.getToken(TokenKey.OMDBToken);
            Console.WriteLine($"Hello World! {omdbToken} {discordToken}");

            client = new DiscordSocketClient();
            commands = new CommandService();
            var service = new ServiceCollection();

            service.AddSingleton(new VotingService());

            services = service.BuildServiceProvider();
            await InstallCommands();

            //  client.Log += Log;
            await client.LoginAsync(Discord.TokenType.Bot, discordToken);
            await client.StartAsync();
            await Task.Delay(-1); 
        }

        public async Task InstallCommands() {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(),services);
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            //check the message for null etc 
            if (message == null) return;
            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}

//https://discord.foxbot.me/docs/guides/getting_started/intro.html
//https://discord.foxbot.me/docs/guides/commands/commands.html
//https://docs.stillu.cc/guides/commands/intro.html