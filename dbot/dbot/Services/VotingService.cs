
using dbot.Data;
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
       // private MovieBotContext _context;

        private IDbManager _dbManager;
     //   private bool _votingOpen; //TODO eventually get this from the Session table instead.

        public VotingService(IDbManager manager)
        {
            _dbManager  = manager;
         //   _votingOpen = false;
        }

        public void Vote(IUser user, int movieId)
        {
            var findNomination = _dbManager.FindNomination(movieId);

            if(findNomination == null)
            {
                Console.WriteLine("Attempting to vote for non existient Nomination");
                return;
            }

            var findUser = _dbManager.FindUser(user);
            if(findUser == null)
            {
                Console.WriteLine($"Could not find user {user.Username}");
                return;
            }

            var newVote = new Vote(findNomination,findUser);
            var currVote = _dbManager.FindVote(user);

            if(currVote==null)
            {
                _dbManager.AddVote(newVote);
            }
            else
            {
                currVote.Nomination = findNomination;
                _dbManager.UpdateNominationInVote(currVote);
            }
        }

        public bool VotingOpen()
        {
            var current = _dbManager.GetSessions().Count();
            if (current ==  0)
                return false;

            return _dbManager.GetCurrentSession().VoteOpen;
            //return _votingOpen;
        }

        public void StartVote()
        {
          //  _votingOpen = true;

            _dbManager.AddSession(new Session() {VoteOpen=true, Timestamp=DateTime.Now});
        }

        public void EndVote()
        {
           // _votingOpen = false;
            
            var currSession =  _dbManager.GetCurrentSession();
            if(currSession == null)
            {
                Console.WriteLine("Current session does not exist. Could not record vote end.");
                return;
            }

            currSession.VoteOpen = false;
            currSession.Timestamp = DateTime.Now;

            _dbManager.UpdateSession(currSession);
        }

        public IEnumerable<VotingResult> GetResults(IEnumerable<Nomination> nominations)
        {
            var results = new List<VotingResult>();

            // Tabulate results
            foreach(var nomination in nominations)
            {
                results.Add(new VotingResult { Movie = nomination,
                                               Votes = _dbManager.GetVotesForNomination(nomination) });
            }
            return results;
        }

        public VotingResult GetWinner(IEnumerable<VotingResult> results)
        {
            var winningVote = results.Select(x => x.Votes).Max();
            var winners = results.Where(x => x.Votes == winningVote);
            var rng = new Random();
            var toSkip = rng.Next(0, winners.Count());
            
            var winner = winners.Skip(toSkip).Take(1).Single();

            var currSess =  _dbManager.GetCurrentSession();
            if(currSess == null)
            {
                Console.WriteLine("Current Session does not exist. Could not update winner.");
                return null;
            }
            currSess.WinningIMDBId=winner.Movie.ImdbId;
            currSess.WinningName=winner.Movie.Name;
            currSess.WinningYear=winner.Movie.Year;
            currSess.NominatedBy=winner.Movie.User.Key;
            _dbManager.UpdateSession(currSess);

            return winner;
        }

        public bool VoteForRandomCandidate(IUser user, IEnumerable<Nomination> nominations)
        {
            if(nominations.Any())
            {
                var rng = new Random();
                var winner = nominations.Skip(rng.Next(0, nominations.Count()))
                                        .Take(1).Single().VotingID;
                Vote(user, winner);
                return true;
            }
            return false;
        }

        public IEnumerable<User> GetVoters()
        {
            return _dbManager.GetVoters().AsEnumerable();
        }

        public void ClearResults()
        {
            _dbManager.ClearVotes();
        }
    }
}