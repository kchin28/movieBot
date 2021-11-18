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

        public User FindUser(IUser user)
        {
            return _context.Users.FirstOrDefault(x => x.Key == user.Username+user.Discriminator);
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            TrySaveChanges();  
        }
        public Nomination FindNomination(int movieId)
        {
            return _context.WeeklyNominations.FirstOrDefault(x => x.VotingID == movieId);
        }
        public Nomination FindNomination(IUser user)
        {
            return _context.WeeklyNominations.Where(x => x.User.Key == user.Username+user.Discriminator).FirstOrDefault();
        }
        public Vote FindVote(IUser user)
        {
            return _context.WeeklyVotes.Where(x => x.User.Key == user.Username+user.Discriminator).FirstOrDefault();
        }

        public void AddVote(Vote newVote)
        {
            _context.WeeklyVotes.Add(newVote);
            TrySaveChanges();
        }

        public void UpdateNominationInVote(Vote vote)
        {
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
            _context.ChangeTracker.Clear();
            _context.Sessions.Add(newSession);
            TrySaveChanges();
        }

        public void UpdateSession(Session updatedSession)
        {
        
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
            return _context.WeeklyNominations.Where(x => x.User.Key == user.Username+user.Discriminator).FirstOrDefault(); 
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
                throw(e);
            }
        }
    }
}