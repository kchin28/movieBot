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
        private IDbManager _dbManager;

        public NominationsService(IDbManager manager)
        {
            _dbManager = manager;
        }
        
        public void AddNomination(IUser user, string title, string imdbId) 
        {
            var findUser = _dbManager.FindUser(user);
            
            if(findUser == null)
            {
                findUser = new User(user);
                _dbManager.AddUser(findUser);
            }

            // Only keeps last nomination
            var newNom = new Nomination(title, GetNextId(),imdbId,findUser);

            var currNom = _dbManager.FindNomination(user);

            if(currNom==null)
            {
                _dbManager.AddNomination(newNom);
            }
            else
            {
                currNom.ImdbId= newNom.ImdbId;
                currNom.Name = newNom.Name;
                currNom.VotingID = newNom.VotingID;

               _dbManager.UpdateNomination(currNom);
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
            return _dbManager.GetNominations().AsEnumerable();
        }

        public void ClearNominations()
        {
            _dbManager.ClearNominations();
        }

        public bool UserHasNomination(IUser user, out Nomination nomination)
        {
            var currentUserNom = _dbManager.GetNomination(user);
            nomination = currentUserNom;
            return currentUserNom == null;
        }

        public void DeleteNominationForUser(IUser user)
        {
            var nom = _dbManager.GetNomination(user);
            if(nom!=null)
            {
                _dbManager.DeleteNomination(nom);
                FixNominationIds();
            }
        }

        private void FixNominationIds()
        {
            // Grab the entire nominations list
            var nominations = _dbManager.GetNominations().ToArray();
            // Re-number from 1 to n of remaining nominations
            int id = 1;

            foreach(var nomination in nominations)
            {
                nomination.VotingID = id++;
            }
            _dbManager.UpdateVotingIDs(nominations);
        }

        private int GetNextId()
        {
            return _dbManager.GetNominations().Count() + 1;
        }
    }
}