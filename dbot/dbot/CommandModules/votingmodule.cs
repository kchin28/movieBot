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
        private readonly NominationsService _nominationsService;
        private readonly OmdbService _omdbService;
        public VotingModule(VotingService votingService, NominationsService nominationsService, OmdbService omdbService)
        {
            _votingService = votingService;
            _nominationsService = nominationsService;
            _omdbService = omdbService;
        }

        [Command("start")]
        public async Task Start()
        {
            if (!_votingService.votingOpen())
            {
                if (_nominationsService.getNominations().Any())
                {
                    _votingService.startVote();
                    await ReplyAsync("Voting has opened!");
                    await ReplyAsync(_nominationsService.viewNominationsWithId());
                }
                else
                {
                    await ReplyAsync("There must be at least one nomination before voting can start!");
                }
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
                var results = _votingService.getResults(_nominationsService.getNominations());
                _votingService.clearResults();
                _nominationsService.clearNominations();
                var sb = new StringBuilder();
                sb.AppendLine("Voting has ended! The results: ");
                foreach (var res in results)
                {
                    sb.AppendLine($"{res.movie.movName}: {res.votes}");
                }
                var winner = _votingService.getWinner(results);
                sb.AppendLine($"The winner is: {winner.movie.movName}");
                await ReplyAsync(sb.ToString());
                var movie = await _omdbService.GetMovieByTitle(winner.movie.movName);
                await ReplyAsync(movie.ToString());
                
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
                if (_nominationsService.getNominations().Select(x => x.id).Contains(movId))
                {
                    _votingService.vote(Context.User, movId);
                    await ReplyAsync($"{Context.User.Username}, your vote has been registered!");
                }
                else
                {
                    await ReplyAsync($"Unexpected vote for {movId}, please try again!");
                }
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("results")]
        public async Task Results()
        {
            var results = _votingService.getResults(_nominationsService.getNominations());
            var sb = new StringBuilder();

            if (_votingService.votingOpen())
            {
                foreach (var res in results)
                {
                    sb.AppendLine($"{res.movie.movName} : {res.votes}");
                }
                await ReplyAsync(sb.ToString());
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("random")]
        public async Task VoteRandom(params int[] candidates)
        {

        }

        [Command("random")]
        public async Task VoteRandom()
        {

        }

        [Command("random")]
        public async Task VoteRandom(params string[] candidates)
        {

        }


    }
}   
