using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbot.Data;
using dbot.Models;
using dbot.Persistence;
using Discord;

namespace dbot.Services
{
    public class NominationsService
    {
        private MovieBotContext _context;

        public NominationsService(MovieBotContext context)
        {
            _context = context;
        }
        
        public void AddNomination(IUser user, string title, string imdbId) 
        {
            // Only keeps last nomination
            var newNom = new Nomination(title, GetNextId(),imdbId,new User(user));

            var currNom = _context.WeeklyNominations.Where(x => x.NominatedBy == user.Username).FirstOrDefault();

            if(currNom==null)
            {
                _context.WeeklyNominations.Add(newNom);
                _context.SaveChanges();
            }
            else
            {
                currNom.ImdbId= newNom.ImdbId;
                currNom.Name = newNom.Name;
                currNom.VotingID = newNom.VotingID;
                _context.Update(currNom);
                _context.SaveChanges();
            }
        }

        public bool IsNominated(string imdbId)
        {
            var nominations = GetNominations();
            if(nominations == null)
                return false;
            return nominations.Where(n => n.ImdbId == imdbId).Any();
        }

        public string ViewNominations()
        {
            var current = GetNominations();
            if(current == null)
                return string.Empty;

            var movies = current.Select(x => x.Name);
            var sb = new StringBuilder();

            foreach (var movie in movies) 
            {
                sb.AppendLine(movie);
            }
            return sb.ToString();
        } 

        public string ViewNominationsWithId()
        {
            var nominations = GetNominations();
            
            if(nominations == null)
                return string.Empty;
        
            var sb = new StringBuilder();

            foreach(var nomination in nominations)
            {
                sb.AppendLine($"{nomination.VotingID}. {nomination.Name}");
            }

            return sb.ToString();
        }

        public IEnumerable<Nomination> GetNominations()
        {
            return _context.WeeklyNominations.Select(x => x);
        }

        public void ClearNominations()
        {
            _context.WeeklyNominations.RemoveRange(_context.WeeklyNominations);
            _context.SaveChanges();
        }

        public bool UserHasNomination(IUser user, out Nomination nomination)
        {
            var currentUserNom = _context.WeeklyNominations.Where(x => x.NominatedBy == user.Username).FirstOrDefault(); 
            nomination = currentUserNom;
            return currentUserNom == null;
        }

        public void DeleteNominationForUser(IUser user)
        {
            var nom = _context.WeeklyNominations.Where(x => x.NominatedBy == user.Username).FirstOrDefault();
            if(nom!=null)
            {
                _context.WeeklyNominations.Remove(nom);
                _context.SaveChanges();
                FixNominationIds();
            }
        }

        private void FixNominationIds()
        {
            // Grab the entire nominations list
            var nominations = _context.WeeklyNominations.ToArray();
            // Re-number from 1 to n of remaining nominations
            int id = 1;

            foreach(var nomination in nominations)
            {
                nomination.VotingID = id++;
            }
            _context.SaveChanges();
        }

        private int GetNextId()
        {
            return _context.WeeklyNominations.Count() + 1;
        }
    }
}