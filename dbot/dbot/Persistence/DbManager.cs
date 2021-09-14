using System;
using System.Collections;
using System.Collections.Generic;
using dbot.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using dbot.Models;
using Discord;

namespace dbot.Persistence
{
    public class DbManager : IDbManager
    {
        private MovieBotContext _context;

        public DbManager(MovieBotContext context)
        {
            _context = context;
        }

        public Nomination FindNomination(int movieId)
        {
            return _context.WeeklyNominations.FirstOrDefault(x => x.VotingID == movieId);
        }

        public Vote FindVote(IUser user)
        {
            return _context.WeeklyVotes.Where(x => x.User.Username == user.Username).FirstOrDefault();
        }

        public void AddVote(Vote newVote)
        {
            _context.WeeklyVotes.Add(newVote);
            _context.SaveChanges();
        }

        public void UpdateNominationInVote(Vote vote, Nomination newNomination)
        {
            vote.Nomination = newNomination;
            _context.Update(vote);
            _context.SaveChanges();
        }

        public List<Session> GetSessions()
        {
            return _context.Sessions.ToList();
        }
        
        public Session GetCurrentSession()
        {
            return _context.Sessions.Last();
        }

        public void AddSession(Session newSession)
        {
            _context.Sessions.Add(newSession);
            _context.SaveChanges();
        }

        public void UpdateSession(Session updatedSession)
        {
            _context.SaveChanges();
        }

        public int GetVotesForNomination(Nomination nomination)
        {
            return _context.WeeklyVotes.Where(x => x.Nomination.VotingID == nomination.VotingID).Count();
        }

        public List<User> GetVoters()
        {
            return _context.WeeklyVotes.Select( x => x.User).ToList();
        }
        
        public void ClearVotes()
        {
            _context.WeeklyVotes.RemoveRange(_context.WeeklyVotes);
            _context.SaveChanges();
        }
    }
}