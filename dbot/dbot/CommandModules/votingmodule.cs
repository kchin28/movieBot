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
        [Priority(3)]
        public async Task Start()
        {
            if (!_votingService.VotingOpen())
            {
                if (_nominationsService.getNominations().Any())
                {
                    _votingService.StartVote();
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
        [Priority(3)]
        public async Task End()
        {
            if (_votingService.VotingOpen())
            {
                _votingService.EndVote();
                var results = _votingService.GetResults(_nominationsService.getNominations());
                _votingService.ClearResults();
                _nominationsService.clearNominations();
                var sb = new StringBuilder();
                sb.AppendLine("Voting has ended! The results: ");
                foreach (var res in results)
                {
                    sb.AppendLine($"{res.movie.movName}: {res.votes}");
                }
                var winner = _votingService.GetWinner(results);
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

        [Command("results")]
        [Priority(2)]
        public async Task Results()
        {
            var results = _votingService.GetResults(_nominationsService.getNominations());
            var sb = new StringBuilder();

            if (_votingService.VotingOpen())
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

        [Command]
        [Priority(2)]
        public async Task Default(int movId) 
        {
            if (_votingService.VotingOpen())
            {
                if (_nominationsService.getNominations().Select(x => x.id).Contains(movId))
                {
                    _votingService.Vote(Context.User, movId);
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

        [Command]
        [Priority(1)]
        public async Task Default([Remainder]string mov)
        {
            if (_votingService.VotingOpen())
            {
                var noms = _nominationsService.getNominations();
                NomObj nomination;
                try
                {
                    nomination = noms.Single(x => x.movName.ToLower().Equals(mov.ToLower()));
                }
                catch 
                {
                    await ReplyAsync($"Unexpected vote for {mov}, please try again!");
                }


                if (!noms.Any())    
                {
                    await ReplyAsync($"Unexpected vote for {mov}, please try again!");
                }
                else 
                {
                    _votingService.Vote(Context.User, nomination.id);
                    await ReplyAsync($"{Context.User.Username}, your vote has been registered!");
                }
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("random")]
        [Priority(3)]
        public async Task VoteRandom()
        {
            if (_votingService.VotingOpen())
            {
                _votingService.VoteForRandomCandidate(Context.User, _nominationsService.getNominations());
                await ReplyAsync($"🎲🎲\r\n{Context.User.Username}, your vote has been registered!");
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("random")]
        [Priority(3)]
        public async Task VoteRandom(params int[] candidates)
        {
            if (_votingService.VotingOpen())
            {
                var nominations = _nominationsService.getNominations().Where(n => candidates.Contains(n.id));
                if(_votingService.VoteForRandomCandidate(Context.User, nominations))
                {
                    await ReplyAsync($"🎲🎲\r\n{Context.User.Username}, your vote has been registered!");
                }
                else
                {
                    await ReplyAsync($"Could not find any valid choices, please try again!");
                }
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("random")]
        [Priority(2)]
        public async Task VoteRandom(params string[] candidates)
        {
            if (_votingService.VotingOpen())
            {
                var nominations = _nominationsService.getNominations().Where(n => candidates.Contains(n.movName));
                if(_votingService.VoteForRandomCandidate(Context.User, nominations))
                {
                    await ReplyAsync($"🎲🎲\r\n{Context.User.Username}, your vote has been registered!");
                }
                else
                {
                    await ReplyAsync($"Could not find any valid choices, please try again!");
                }
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

    }
}   
