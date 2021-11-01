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
        public Nomination FindNomination(IUser user)
        {
            return _context.WeeklyNominations.Where(x => x.User.Username == user.Username).FirstOrDefault();
        }
        public Vote FindVote(IUser user)
        {
            return _context.WeeklyVotes.Where(x => x.User.Username == user.Username).FirstOrDefault();
        }

        public void AddVote(Vote newVote)
        {
            _context.WeeklyVotes.Add(newVote);
          TrySaveChanges();
        }

        public void UpdateNominationInVote(Vote vote)
        {
            _context.Entry(vote).State = EntityState.Modified;
           TrySaveChanges();
        }

        public List<Session> GetSessions()
        {
            return _context.Sessions.ToList();
        }
        
        public Session GetCurrentSession()
        {
            if(_context.Sessions?.Count()==0)
                return null;

            return _context.Sessions.OrderBy(x => x.Timestamp).Last();
        }

        public void AddSession(Session newSession)
        {
            _context.Sessions.Add(newSession);
            TrySaveChanges();
        }

        public void UpdateSession(Session updatedSession)
        {
            _context.Entry(updatedSession).State = EntityState.Modified;
            TrySaveChanges();
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
           TrySaveChanges();
        }

        public void AddNomination(Nomination newNom)
        {
             _context.WeeklyNominations.Add(newNom);
           TrySaveChanges();
        }

        public void UpdateNomination(Nomination currNom)
        {
            _context.Entry(currNom.ImdbId).State = EntityState.Modified;
            _context.Entry(currNom.Name).State = EntityState.Modified;
            _context.Entry(currNom.VotingID).State = EntityState.Modified;
           TrySaveChanges();
        }

        public List<Nomination> GetNominations()
        {
            return _context.WeeklyNominations.Select(x => x).ToList();
        }

        public void ClearNominations()
        {
            _context.WeeklyNominations.RemoveRange(_context.WeeklyNominations);
            TrySaveChanges();
        }

        public Nomination GetNomination(IUser user)
        {
            return _context.WeeklyNominations.Where(x => x.User.Username == user.Username).FirstOrDefault(); 
        }

        public void DeleteNomination(Nomination nom)
        {
            _context.WeeklyNominations.Remove(nom);
            TrySaveChanges();
        }

        public void UpdateVotingIDs(IEnumerable<Nomination> nominations)
        {
            TrySaveChanges();
        }

        private void TrySaveChanges()
        {
            try
            {
            _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine($"Inner Exception: {e.InnerException}"); 
            }
        }
    }
}