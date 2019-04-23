using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace dbot.Services
{
    public class NominationsService
    {
        private static ConcurrentDictionary<IUser, Nomination> currNoms = new ConcurrentDictionary<IUser, Nomination>();

        public void AddNomination(IUser userName,string title, string imdbID) 
        {

                //only keeps last nomination
                var newNom = new Nomination(title, getNextId(),imdbID);
                currNoms.AddOrUpdate(userName, newNom,
                    (k, v) =>
                    {
                        v.movName = newNom.movName;
                        return v;
                    });
        }

        public bool IsNominated(string imdbId)
        {
            var nominations = getNominations();
            return nominations.Where(n => n.imdb == imdbId).Any();
        }

        public string viewNominations() {
            var current = getNominations();
            var movies = current.Select(x => x.movName);
            var sb = new StringBuilder();

            foreach (var movie in movies) {
                sb.AppendLine(movie);
            }
            return sb.ToString();
        } 

        public string viewNominationsWithId()
        {
            var nominations = getNominations();

            var sb = new StringBuilder();

            foreach(var nomination in nominations)
            {
                sb.AppendLine($"{nomination.id}. {nomination.movName}");
            }
            return sb.ToString();
        }

        public IEnumerable<Nomination> getNominations()
        {
            return currNoms.Select(x => x.Value).OrderBy(x => x.id);
        }

        public void clearNominations()
        {
            currNoms.Clear();
        }

        public bool UserHasNomination(IUser user, out Nomination nomination)
        {
            return currNoms.TryGetValue(user, out nomination);
        }

        public void DeleteNominationForUser(IUser user)
        {
            Nomination nomination;
            if(currNoms.TryRemove(user, out nomination))
            {
                FixNominationIds();
            }
        }

        private void FixNominationIds()
        {
            //Grab the entire nominations list
            var nominations = currNoms.ToArray();
            //Re-number from 1 to n of remaining nominations
            int id = 1;
            foreach(var nomination in nominations)
            {
                nomination.Value.id = id++;
                currNoms.AddOrUpdate(nomination.Key, nomination.Value, (k, v) => { return nomination.Value; });
            }
        }

        private int getNextId()
        {
            return currNoms.Count() + 1;
        }
        //cannot delete nominations only replace bc the id's won't be in order
        //no verification on omdb
    }

    public class Nomination 
    {
        public string movName;
        public int id;
        public string imdb;

        public Nomination(string movie, int num, string imdbID) 
        {
            movName = movie;
            id = num;
            imdb = imdbID;
        }

    }
}