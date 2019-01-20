using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dbot.Services;
using System.Linq;

namespace dbot.CommandModules
{
    [Group("vote")]
    public class VotingModule : ModuleBase
    {
        private readonly VotingService _votingService;
        public VotingModule(VotingService votingService)
        {
            _votingService = votingService;
        }

        [Command("start")]
        public async Task Start()
        {
            if (!_votingService.votingOpen())
            {
                _votingService.startVote();
                await ReplyAsync("Voting has opened!");
            }
            else
            {
                await ReplyAsync("There is already a vote in progress!");
            }
        }

        [Command("end")]
        public async Task End()
        {
            if (_votingService.votingOpen())
            {
                _votingService.endVote();
                var results = _votingService.getResults();
                _votingService.clearResults();
                var sb = new StringBuilder();
                sb.AppendLine("Voting has ended! The results: ");
                foreach (var res in results)
                {
                    sb.AppendLine($"{res.name} : {res.votes}");
                }
                var winner = results.OrderByDescending(x => x.votes);
                sb.AppendLine($"The winner is: {winner.First().name}");
                await ReplyAsync(sb.ToString());
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command]
        public async Task Default(int movId) 
        {
            if (_votingService.votingOpen())
            {
                _votingService.vote(Context.User, movId);
                await ReplyAsync($"{Context.User.Username}, your vote has been registered!");
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("results")]
        public async Task Results()
        {
            var results = _votingService.getResults();
            var sb = new StringBuilder();

            if (_votingService.votingOpen())
            {
                foreach (var res in results)
                {
                    sb.AppendLine($"{res.name} : {res.votes}");
                }
                await ReplyAsync(sb.ToString());
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }


    }
}   
