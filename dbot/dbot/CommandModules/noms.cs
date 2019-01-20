using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using dbot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dbot.CommandModules
{
    public class noms : ModuleBase
    {
        private readonly NominationsService _nominationsService;
        private readonly OmdbService _omdbService;


        public noms(NominationsService ns, OmdbService os)
        {
            _nominationsService = ns;
            _omdbService = os;
        }

        [Command("nominate")]
        public async Task addNominationASync(string name)
        {
            var movie = await _omdbService.GetMovieByTitle(name);

            if (movie.Title.Equals(null))
            {
                await ReplyAsync("Could not find this movie.");
            }
            else {
                var sb = new StringBuilder();
                sb.Append(movie.Title);
                sb.Append(movie.Year);
                sb.Append(movie.Plot);
                sb.Append(movie.Poster);
                await ReplyAsync(sb.ToString());

                //if this isnt the right one, specify the year and change the nomination
                _nominationsService.addNom(Context.User, name, movie.imdbID);
                await ReplyAsync("Thanks for nominating!");
            }
        }

        [Command("nominate")]
        public async Task addNominationWithYearASync(string name, int year) {
            var movie = await _omdbService.GetMovieByTitleYear(name,year);

            if (movie.Title.Equals(null))
            {
                await ReplyAsync("Could not find this movie.");
            }
            else
            {
                await ReplyAsync(movie.ToString());

                //if this isnt the right one, specify the year and change the nomination
                _nominationsService.addNom(Context.User, name, movie.imdbID);
                await ReplyAsync("Thanks for nominating!");
            }
        }

        [Command("nominations")]
        public async Task viewNominationsAsynch() {
            string result = _nominationsService.viewNominations();
            await ReplyAsync(result);

        }

    }
}   
