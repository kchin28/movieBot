using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dbot.Services;
using dbot.Models;
using System.Linq;

namespace dbot.CommandModules
{
    [Group("Vote")]
    [Summary("Commands for running a round of voting")]
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

        [Command("Start")]
        [Summary("Starts a round of voting")]
        [Remarks("Usage: !vote start")]
        [Priority(3)]
        public async Task Start()
        {
            if (!_votingService.VotingOpen())
            {
                if (_nominationsService.GetNominations().Any())
                {
                    _votingService.StartVote();
                    Console.WriteLine("Starting voting session");

                    await ReplyAsync("Voting has opened!");
                    await ReplyAsync(_nominationsService.ViewNominationsWithId());
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

        [Command("End")]
        [Summary ("Ends the round of voting and prints the results")]
        [Remarks("Usage: !vote end")]
        [Priority(3)]
        public async Task End()
        {
            if (_votingService.VotingOpen())
            {
                Console.WriteLine("Ending voting session");
                _votingService.EndVote();

                var results = _votingService.GetResults(_nominationsService.GetNominations());
                _votingService.ClearResults();
                _nominationsService.ClearNominations();

                var sb = new StringBuilder();
                sb.AppendLine("Voting has ended! The results: ");

                foreach (var res in results)
                {
                    sb.AppendLine($"{res.Movie.Name}: {res.Votes}");
                }

                var winner = _votingService.GetWinner(results);

                sb.AppendLine($"The winner is: {winner.Movie.Name}");

                await ReplyAsync(sb.ToString());
                var movie = await _omdbService.GetMovieById(winner.Movie.ImdbId);
                await ReplyAsync(movie.ToString());
                
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("Results")]
        [Summary ("Prints the in progress results from a round of voting")]
        [Remarks("Usage: !vote results")]
        [Priority(2)]
        public async Task Results()
        {
            Console.WriteLine("Got request to show vote results");
            var results = _votingService.GetResults(_nominationsService.GetNominations());
            var sb = new StringBuilder();

            if (_votingService.VotingOpen())
            {
                foreach (var res in results)
                {
                    sb.AppendLine($"{res.Movie.Name} : {res.Votes}");
                }
                await ReplyAsync(sb.ToString());
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command]
        [Name("By Id")]
        [Summary("Votes for a movie by assigned id")]
        [Remarks("Usage: !vote <integer>")]
        [Priority(2)]
        public async Task Default(int movId)
        {
            Console.WriteLine("Got vote");
            if (_votingService.VotingOpen())
            {
                if (_nominationsService.GetNominations().Select(x => x.VotingID).Contains(movId))
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
        [Name("By Name")]
        [Summary("Votes for a movie by movie name")]
        [Remarks("Usage: !vote <movie name>")]
        [Priority(1)]
        public async Task Default([Remainder]string mov)
        {
            Console.WriteLine("Got vote");
            if (_votingService.VotingOpen())
            {
                var noms = _nominationsService.GetNominations();
                Nomination nomination = null;

                try
                {
                    nomination = noms.Single(x => x.Name.ToLower().Equals(mov.ToLower()));
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
                    _votingService.Vote(Context.User, nomination.VotingID);
                    await ReplyAsync($"{Context.User.Username}, your vote has been registered!");
                }
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("Random")]
        [Name("True Random")]
        [Summary("Votes for a movie randomly")]
        [Remarks("Usage: !vote random")]
        [Priority(3)]
        public async Task VoteRandom()
        {
            Console.WriteLine("Got random vote");

            if (_votingService.VotingOpen())
            {
                _votingService.VoteForRandomCandidate(Context.User, _nominationsService.GetNominations());
                await ReplyAsync($"🎲🎲\r\n{Context.User.Username}, your vote has been registered!");
            }
            else
            {
                await ReplyAsync($"There is no vote in progress");
            }
        }

        [Command("random")]
        [Name("Random Id List")]
        [Summary("Votes for a movie randomly from a list of space separated ids")]
        [Remarks("Usage: !vote random <id1> <id2> ..")]
        [Priority(3)]
        public async Task VoteRandom(params int[] candidates)
        {
            Console.WriteLine("Got half-assed random vote");
            if (_votingService.VotingOpen())
            {
                var nominations = _nominationsService.GetNominations().Where(n => candidates.Contains(n.VotingID));

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
        [Name("Random Name List")]
        [Summary("Votes for a movie randomly from a list of space separated movie names")]
        [Remarks("Usage: !vote random \"<movie1>\" \"<movie2>\" ..")]
        [Priority(2)]
        public async Task VoteRandom(params string[] candidates)
        {
            Console.WriteLine("Got half-assed random vote");
            if (_votingService.VotingOpen())
            {
                var nominations = _nominationsService.GetNominations().Where(n => candidates.Contains(n.Name));

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
