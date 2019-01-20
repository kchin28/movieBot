using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Discord;

namespace dbot.Services
{
    public class NominationsService
    {
        private static ConcurrentDictionary<IUser, NomObj> currNoms = new ConcurrentDictionary<IUser, NomObj>();

        public void addNom(IUser userName,string title) { //no omdb verification currently
                //only keeps last nomination
                var newNom = new NomObj(title, currNoms.Count+1);
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

        //cannot delete nominations only replace bc the id's won't be in order
        //no verification on omdb
    }

    public class NomObj {
        public string movName;
        public int id;

        public NomObj(string movie, int num) {
            movName = movie;
            id = num;

        }

    }
}