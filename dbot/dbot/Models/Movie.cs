using System;
using System.Collections.Generic;
using System.Text;

namespace dbot.Models
{
    public class Movie
    {
        public string Title {get; set;}
        public string Year {get; set;}
        public string Rated {get; set;}
        public string Released {get; set;}
        public string Runtime {get; set;}
        public string Genre {get; set;}
        public string Director {get; set;}
        public string Writer {get; set;}
        public string Actors {get; set;}
        public string Plot {get; set;}
        public string Language {get; set;}
        public string Country {get; set;}
        public string Awards {get; set;}
        public string Poster {get; set;}
        public List<Rating> Ratings {get; set;}
        public string Metascore {get; set;}
        public string ImdbRating {get; set;}
        public string ImdbVotes {get; set;}
        public string ImdbId {get; set;}
        public string Type {get; set;}
        public string DVD {get; set;}
        public string BoxOffice {get; set;}
        public string Production {get; set;}
        public string Website {get; set;}
        public string Response {get; set;}

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"**{Title} - ({Year}) - {Runtime}**");
            sb.AppendLine($"{Plot}");
            
            foreach(var rating in Ratings)
            {
                sb.AppendLine($"{rating.Source} - {rating.Value}");
            }

            sb.AppendLine($"{Poster}");
            return sb.ToString();
        }
    }
}
