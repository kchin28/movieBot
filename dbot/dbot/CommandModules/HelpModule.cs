using dbot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dbot.CommandModules
{
    [Group("help")]
    class HelpModule : ModuleBase
    {
        private CommandService _commandService;
        public HelpModule(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command]
        public async Task Default()
        {
            await ReplyAsync("Help me. Save me.");
        }
    }
}
