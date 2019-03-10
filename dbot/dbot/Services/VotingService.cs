
using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace dbot.Services
{
    public class VotingResult
    {
        public Nomination movie { get; set; }
        public int votes { get; set; }
    }

    public class VotingService
    {
        private ConcurrentDictionary<IUser, int> _votes;

        private bool _votingOpen;

        public VotingService()
        {
            _votingOpen = false;
            _votes = new ConcurrentDictionary<IUser, int>();
        }

        public void Vote(IUser user, int movieId)
        {
            //Always update with newest movieId
            _votes.AddOrUpdate(user, movieId,
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
            _votingOpen = true;
        }

        public void EndVote()
        {
            _votingOpen = false;
        }

        public IEnumerable<VotingResult> GetResults(IEnumerable<Nomination> nominations)
        {
            var results = new List<VotingResult>();

            //Tabulate results
            foreach(var nomination in nominations)
            {
                results.Add(new VotingResult { movie = nomination,
                                               votes = _votes.Values.Where(x => x == nomination.id).Count() });
            }
            return results;
        }

        public VotingResult GetWinner(IEnumerable<VotingResult> results)
        {
            var winningVote = results.Select(x => x.votes).Max();
            var winners = results.Where(x => x.votes == winningVote);
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
                                        .Take(1).Single().id;
                Vote(user, winner);
                return true;
            }
            return false;
        }

        public void ClearResults()
        {
            _votes.Clear();
        }
    }
}