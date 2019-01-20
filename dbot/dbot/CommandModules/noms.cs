using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using dbot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dbot.CommandModules
{
    public class noms : ModuleBase
    {
        private readonly NominationsService _nominationsService;

        public noms(NominationsService ns)
        {
            _nominationsService = ns;
        }
        public async Task addNomination()
        {
            var success = _nominationsService.addNom();
            await ReplyAsync();
        }
    }
}   
