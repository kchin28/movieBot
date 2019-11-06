using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbot.Models;
using dbot.Persistence;
using Discord;

namespace dbot.Services
{
    public class NominationsService
    {
        private readonly IRepository<User, Nomination> currNoms;

        public NominationsService(IRepository<User, Nomination> nominations)
        {
            currNoms = nominations;
        }

        public void AddNomination(IUser user, string title, string imdbId) 
        {
            // Only keeps last nomination
            var newNom = new Nomination(title, GetNextId(),imdbId);

            currNoms.AddOrUpdate(new User(user), newNom,
                (k, v) =>
                {
                    v.ImdbId  = newNom.ImdbId;
                    v.Name    = newNom.Name;
                    return v;
                });
        }

        public bool IsNominated(string imdbId)
        {
            var nominations = GetNominations();
            return nominations.Where(n => n.ImdbId == imdbId).Any();
        }

        public string ViewNominations()
        {
            var current = GetNominations();
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
            var sb = new StringBuilder();

            foreach(var nomination in nominations)
            {
                sb.AppendLine($"{nomination.VotingId}. {nomination.Name}");
            }

            return sb.ToString();
        }

        public IEnumerable<Nomination> GetNominations()
        {
            return currNoms.Select(x => x.Value).OrderBy(x => x.VotingId);
        }

        public void ClearNominations()
        {
            currNoms.Clear();
        }

        public bool UserHasNomination(IUser user, out Nomination nomination)
        {
            return currNoms.TryGetValue(new User(user), out nomination);
        }

        public void DeleteNominationForUser(IUser user)
        {
            if (currNoms.TryRemove(new User(user), out _))
            {
                FixNominationIds();
            }
        }

        private void FixNominationIds()
        {
            // Grab the entire nominations list
            var nominations = currNoms.ToArray();
            // Re-number from 1 to n of remaining nominations
            int id = 1;

            foreach(var nomination in nominations)
            {
                nomination.Value.VotingId = id++;
                currNoms.AddOrUpdate(nomination.Key, nomination.Value, (k, v) => { return nomination.Value; });
            }
        }

        private int GetNextId()
        {
            return currNoms.Count() + 1;
        }
    }

    public class Nomination 
    {
        public string Name;
        public int VotingId;
        public string ImdbId;

        public Nomination(string name, int votingId, string imdbId) 
        {
            Name = name;
            VotingId = votingId;
            ImdbId = imdbId;
        }

    }
}