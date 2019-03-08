using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace dbot.Services
{
    public class NominationsService
    {
        private static ConcurrentDictionary<IUser, NomObj> currNoms = new ConcurrentDictionary<IUser, NomObj>();

        public void addNom(IUser userName,string title, string imdbID) { //no omdb verification currently 

                //only keeps last nomination
                var newNom = new NomObj(title, currNoms.Count+1,imdbID);
                currNoms.AddOrUpdate(userName, newNom,
                    (k, v) =>
                    {
                        v.movName = newNom.movName;
                        return v;
                    });
        }

        public string viewNominations() {
            var current = currNoms.Values;
            var movies = current.Select(x => x.movName);
            var sb = new StringBuilder();

            foreach (var movie in movies) {
                sb.AppendLine(movie);
            }
            return sb.ToString();
        } 

        public string viewNominationsWithId()
        {
            var nominations = currNoms.Values;

            var sb = new StringBuilder();

            foreach(var nomination in nominations)
            {
                sb.AppendLine($"{nomination.id}. {nomination.movName}");
            }
            return sb.ToString();
        }

        public IEnumerable<NomObj> getNominations()
        {
            return currNoms.Select(x => x.Value).OrderBy(x => x.id);
        }

        public void clearNominations()
        {
            currNoms.Clear();
        }

        //cannot delete nominations only replace bc the id's won't be in order
        //no verification on omdb
    }

    public class NomObj {
        public string movName;
        public int id;
        public string imdb;

        public NomObj(string movie, int num, string imdbID) {
            movName = movie;
            id = num;
            imdb = imdbID;
        }

    }
}