
using dbot.Models;
using dbot.Persistence;
using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace dbot.Services
{
    public class VotingResult
    {
        public Nomination Movie { get; set; }
        public int Votes { get; set; }
    }

    public class VotingService
    {
        private readonly IRepository<User, int> _votes;

        private bool _votingOpen;

        public VotingService(IRepository<User, int> votes)
        {
            _votes = votes;
            _votingOpen = votes.Any();
        }

        public void Vote(IUser user, int movieId)
        {
            // Always update with newest movieId
            _votes.AddOrUpdate(new User(user), movieId,
                (key, movie) =>
                {
                    return movieId;
                });
        }

        public bool VotingOpen()
        {
            return _votingOpen;
        }

        public void StartVote()
        {
            _votes.Clear();
            _votingOpen = true;
        }

        public void EndVote()
        {
            _votes.Clear();
            _votingOpen = false;
        }

        public IEnumerable<VotingResult> GetResults(IEnumerable<Nomination> nominations)
        {
            var results = new List<VotingResult>();

            // Tabulate results
            foreach(var nomination in nominations)
            {
                results.Add(new VotingResult { Movie = nomination,
                                               Votes = _votes.Values.Where(x => x == nomination.VotingId).Count() });
            }
            return results;
        }

        public VotingResult GetWinner(IEnumerable<VotingResult> results)
        {
            var winningVote = results.Select(x => x.Votes).Max();
            var winners = results.Where(x => x.Votes == winningVote);
            var rng = new Random();
            var toSkip = rng.Next(0, winners.Count());
            
            return winners.Skip(toSkip).Take(1).Single();
        }

        public bool VoteForRandomCandidate(IUser user, IEnumerable<Nomination> nominations)
        {
            if(nominations.Any())
            {
                var rng = new Random();
                var winner = nominations.Skip(rng.Next(0, nominations.Count()))
                                        .Take(1).Single().VotingId;
                Vote(user, winner);
                return true;
            }
            return false;
        }

        public IEnumerable<User> GetVoters()
        {
            return _votes.Keys;
        }
    }
}