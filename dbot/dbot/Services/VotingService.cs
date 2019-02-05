
using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace dbot.Services
{
    public class VotingResult
    {
        public NomObj movie { get; set; }
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

        public void vote(IUser user, int movieId)
        {
            //Always update with newest movieId
            _votes.AddOrUpdate(user, movieId,
                (key, movie) =>
                {
                    return movieId;
                });
        }

        public bool votingOpen()
        {
            return _votingOpen;
        }

        public void startVote()
        {
            _votingOpen = true;
        }

        public void endVote()
        {
            _votingOpen = false;
        }

        public IEnumerable<VotingResult> getResults(IEnumerable<NomObj> nominations)
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

        public VotingResult getWinner(IEnumerable<VotingResult> results)
        {
            var winningVote = results.Select(x => x.votes).Max();
            var winners = results.Where(x => x.votes == winningVote);
            var rng = new Random();
            var toSkip = rng.Next(0, winners.Count());
            
            return winners.Skip(toSkip).Take(1).Single();
        }

        public void clearResults()
        {
            _votes.Clear();
        }
    }
}