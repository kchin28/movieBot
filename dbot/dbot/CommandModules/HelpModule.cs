using dbot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbot.CommandModules
{
    [Group("Help")]
    [Summary("Help information")]
    class HelpModule : ModuleBase
    {
        private CommandService _commandService;
        public HelpModule(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command]
        [Name("General")]
        [Summary ("Prints all available commands and their descriptions")]
        [Remarks("Usage: !help")]
        public async Task Default()
        {
            var modules = _commandService.Modules;
            var sb = new StringBuilder();
            sb.AppendLine("Commands:");
            foreach(var module in modules)
            {
                sb.AppendLine($"**{module.Name}**: {module.Summary}");
            }
            await ReplyAsync(sb.ToString());
        }
        
        [Command]
        [Name("Details")]
        [Summary("Prints information about a specific command")]
        [Remarks("Usage: !help <command>")]
        public async Task Default([Remainder]string commandName)
        {
            try
            {
                var module = _commandService.Modules.Where(m => m.Name.ToLower() == commandName.ToLower())
                                                    .Single();

                var sb = new StringBuilder();
                sb.AppendLine($"Usage information for {module.Name}");
                foreach(var command in module.Commands)
                {
                    sb.AppendLine($"**{command.Name}**: {command.Summary}");
                    sb.AppendLine($"{command.Remarks}");
                    //Line separation for readability
                    sb.AppendLine();
                }
                await ReplyAsync(sb.ToString());
            }
            catch
            {
                await ReplyAsync("Command not found!");
            }
        }
    }
}
