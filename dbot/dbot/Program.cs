using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using dbot.Services;
using dbot.CommandModules;

namespace dbot
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;
        private IServiceCollection serviceCollection;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            var discordToken = TokenManager.GetToken(TokenKey.DiscordToken);
            var omdbToken = TokenManager.GetToken(TokenKey.OMDBToken);
            Console.WriteLine($"Hello World! {omdbToken} {discordToken}");

            client = new DiscordSocketClient();
            commands = new CommandService();
            serviceCollection = new ServiceCollection();
          
            serviceCollection.AddSingleton(new NominationsService());
            serviceCollection.AddSingleton(new VotingService());
            serviceCollection.AddSingleton(new OmdbService(omdbToken));
            serviceCollection.AddSingleton(commands);

            services = serviceCollection.BuildServiceProvider();
            await InstallCommands();

            //  client.Log += Log;
            await client.LoginAsync(Discord.TokenType.Bot, discordToken);
            await client.StartAsync();
            await Task.Delay(-1); 
        }

        public async Task InstallCommands() {
            client.MessageReceived += HandleCommand;
            await commands.AddModuleAsync<VotingModule>(services);
            await commands.AddModuleAsync<NominationsModule>(services);
            await commands.AddModuleAsync<ViewModule>(services);
            await commands.AddModuleAsync<InfoModule>(services);
            await commands.AddModuleAsync<HelpModule>(services);
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
            {
                Console.WriteLine($"Command failed: {result.ErrorReason}; {result.Error} (Message: {messageParam})");
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}

//https://discord.foxbot.me/docs/guides/getting_started/intro.html
//https://discord.foxbot.me/docs/guides/commands/commands.html
//https://docs.stillu.cc/guides/commands/intro.html