
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
        private MovieBotContext _context;
        private bool _votingOpen; //TODO eventually get this from the Session table instead.

        public VotingService(MovieBotContext context)
        {
            _context = context;
            _votingOpen = false;
        }

        public void Vote(IUser user, int movieId)
        {
            var findNomination = _context.WeeklyNominations.FirstOrDefault(x => x.VotingID == movieId);

            if(findNomination == null)
            {
                return;
            }

            var newVote = new Vote(findNomination,new User(user));
            var currVote = _context.WeeklyVotes.Where(x => x.User.Username == user.Username).FirstOrDefault();

            if(currVote==null)
            {
                _context.WeeklyVotes.Add(newVote);
                _context.SaveChanges();
            }
            else
            {
                currVote.Nomination = findNomination;
                _context.Update(currVote);
                _context.SaveChanges();
            }
        }

        public bool VotingOpen()
        {
            var current = _context.Sessions.Count();
            if (current ==  0)
                return false;
            return _context.Sessions.Last().VoteOpen;
            //return _votingOpen;
        }

        public void StartVote()
        {
            _votingOpen = true;

            _context.Sessions.Add(new Session() {VoteOpen=true, });
            _context.SaveChanges();
        }

        public void EndVote()
        {
            _votingOpen = false;
            
            var currSession =  _context.Sessions.Last();
            currSession.VoteOpen = false;
            currSession.Timestamp = DateTime.Now;
            _context.SaveChanges();
        }

        public IEnumerable<VotingResult> GetResults(IEnumerable<Nomination> nominations)
        {
            var results = new List<VotingResult>();

            // Tabulate results
            foreach(var nomination in nominations)
            {
                results.Add(new VotingResult { Movie = nomination,
                                               Votes = _context.WeeklyVotes.Where(x => x.Nomination.VotingID == nomination.VotingID).Count() });
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

            var currSess =  _context.Sessions.Last();
            currSess.WinningIMDBId=winner.Movie.ImdbId;
            currSess.WinningName=winner.Movie.Name;
            currSess.WinningYear=winner.Movie.Year;
            currSess.NominatedBy=winner.Movie.User.Username;
            _context.SaveChanges();

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
            return _context.WeeklyVotes.Select( x => x.User);
        }

        public void ClearResults()
        {
              _context.WeeklyVotes.RemoveRange(_context.WeeklyVotes);
              _context.SaveChanges();
        }
    }
}