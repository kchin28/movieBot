using dbot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dbot.CommandModules
{
    [Group("Info")]
    [Summary("Commands for fetching movie information")]
    class InfoModule : ModuleBase
    {
        private readonly OmdbService _omdbService;
        public InfoModule(OmdbService omdbService)
        {
            _omdbService = omdbService;
        }

        [Command]
        [Name("By Name")]
        [Summary("Searches for information of a movie by name")]
        [Remarks("Usage: !info <movie name>")]
        public async Task InfoByName([Remainder]string movieName)
        {
            var movie = await _omdbService.GetMovieByTitle(movieName);
            Console.WriteLine($"Retrieved movie info for \"{movieName}\"; id={movie.ImdbId}");
            await ReplyAsync(movie.ToString());
        }

        [Command("year")]
        [Name("By Year")]
        [Summary("Searches for information of a movie by name and year")]
        [Remarks("Usage: !info year \"<movie name>\" <year>")]
        public async Task InfoByYear(string movieName, int year)
        {
            var movie = await _omdbService.GetMovieByTitleYear(movieName, year);
            Console.WriteLine($"Retrieved movie info for \"{movieName}\", {year}; id={movie.ImdbId}");
            await ReplyAsync(movie.ToString());
        }

        [Command("id")]
        [Name("By IMDB Id")]
        [Summary("Searches for information of a movie by IMDB id")]
        [Remarks("Usage: !info id <id>")]
        [Priority(1)]
        public async Task InfoById(string id)
        {
            var movie = await _omdbService.GetMovieById(id);
            Console.WriteLine($"Retrieved movie info for id={movie.ImdbId}");
            await ReplyAsync(movie.ToString());
        }
    }
}
