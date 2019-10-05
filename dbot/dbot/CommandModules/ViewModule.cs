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
    [Group("View")]
    [Summary("Commands for viewing nomination and voting state")]
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

        [Command("Nominations")]
        [Summary("Prints a list of all current nominations")]
        [Remarks("Usage: !view nominations")]
        public async Task ViewNomination()
        {
            Console.WriteLine("Got view nominations request");
            var noms = _nominationsService.GetNominations();
            
            if (!noms.Any())
            {
                return;
            }

            var sb = new StringBuilder();

            foreach (var n in noms) 
            {
                sb.AppendLine($"{n.VotingId}. {n.Name}");
            }

            await ReplyAsync(sb.ToString());
        }

        [Command("Votes")]
        [Summary("Prints the current voting status")]
        [Remarks("Usage: !view votes")]
        public async Task ViewVotes()
        {
            Console.WriteLine("Got view votes request");

            var votes = _votingService.GetResults(_nominationsService.GetNominations());
            var sb = new StringBuilder();

            foreach (var v in votes)
            {
                sb.AppendLine($"{v.Movie.VotingId}. {v.Movie.Name}: {v.Votes}");
            }

            await ReplyAsync(sb.ToString());

        }
        [Command("Voters")]
        [Summary("Prints a list of all users who have voted")]
        [Remarks("Usage: !view voters")]
        public async Task ViewVoters()
        {
            Console.WriteLine("Got view voters request");
            var voters = _votingService.GetVoters();

            var sb = new StringBuilder();
            sb.AppendLine("Current Voters:");

            foreach (var u in voters)
            {
                sb.AppendLine($"{u.Username}");
            }

            await ReplyAsync(sb.ToString());
        }

    }
}
