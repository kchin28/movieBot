
using Discord;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace dbot.Services
{
    public class VotingResult
    {
        public string name { get; set; }
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

        public IEnumerable<VotingResult> getResults()
        {
            var results = new List<VotingResult>();

            //For movie in nominationservice
            //Tabulate results
            for(int i = 0; i < 10; i++)
            {
                results.Add(new VotingResult { name = $"test{i}", votes = _votes.Values.Where(x => x == i).Count() });
            }
            return results;
        }

        public void clearResults()
        {
            int ignore;
            foreach(var key in _votes.Keys)
            {
                _votes.Remove(key, out ignore);
            }
        }
    }
}