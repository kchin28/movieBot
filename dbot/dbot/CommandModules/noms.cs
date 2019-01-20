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

        [Command("nominate")]
        public async Task addNominationASync(string name)
        {
            _nominationsService.addNom(Context.User,name);
            await ReplyAsync("thanks for nominating");
        }

        [Command("nominations")]
        public async Task viewNominationsAsynch() {
            string result = _nominationsService.viewNominations();
            await ReplyAsync(result);

        }
    }
}   
