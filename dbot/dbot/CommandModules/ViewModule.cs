using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dbot.Services;
using System.Threading.Tasks;
using System.Linq;

namespace dbot.CommandModules
{
    [Group("view")]
    public class ViewModule : ModuleBase
    {
        private readonly NominationsService _nominationsService;
        private readonly OmdbService _omdbService;
        private readonly VotingService _votingService;

        public ViewModule(NominationsService ns, OmdbService os, VotingService vs)
        {
            _nominationsService = ns;
            _omdbService = os;
            _votingService = vs;
        }

        [Command("nominations")]
        public async Task viewNom() {
            var noms = _nominationsService.getNominations();
            if (!noms.Any()) { return; }
            StringBuilder sb = new StringBuilder();
            foreach (var n in noms) {
                sb.AppendLine($"{n.id}. {n.movName}");
            }
            await ReplyAsync(sb.ToString());
        }

        [Command("votes")]
        public async Task viewVotes()
        {
            var votes = _votingService.getResults(_nominationsService.getNominations());
            StringBuilder sb = new StringBuilder();
            foreach (var v in votes)
            {
                sb.AppendLine($"{v.movie.id}. {v.movie.movName}: {v.votes}");
            }
            await ReplyAsync(sb.ToString());

        }

    }
}
