using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using dbot.Services;
using dbot.CommandModules;
using dbot.Persistence;
using dbot.Models;
using dbot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace dbot
{
    class Program
    {
        private const string DBCONN = "Data Source=movieBot.db";

        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;
        private IServiceCollection serviceCollection;
        static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();


        public async Task MainAsync(string[] args)
        {
            TokenManager tokenManager;
            if (args.Length == 0)
            {
                tokenManager = new TokenManager();
            }
            else
            {
                tokenManager = new TokenManager(args[0]);
            }

            var discordToken = tokenManager.GetToken(TokenKey.DiscordToken);
            var omdbToken = tokenManager.GetToken(TokenKey.OMDBToken);
         //   var nominationsFile = tokenManager.GetToken(TokenKey.NominationsFile);
     //       var votesFile = tokenManager.GetToken(TokenKey.VotesFile);
            Console.WriteLine($"Hello World! {omdbToken} {discordToken}");

            client = new DiscordSocketClient();
            commands = new CommandService();
            serviceCollection = new ServiceCollection();
          
            serviceCollection.AddDbContext<MovieBotContext>(options => options.UseSqlite(DBCONN));
            serviceCollection.AddScoped<IDbManager,DbManager>();
            serviceCollection.AddScoped<NominationsService>();
            serviceCollection.AddScoped<VotingService>();
            serviceCollection.AddSingleton(new OmdbService(omdbToken));
            serviceCollection.AddSingleton(commands);

            services = serviceCollection.BuildServiceProvider();

            //check for db and create it if it does not exist
            try
            {
                var context = services.GetRequiredService<MovieBotContext>();
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong during DB initialize: " + ex.Message);
            }

            await InstallCommands();

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

            // Check the message for null etc 
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