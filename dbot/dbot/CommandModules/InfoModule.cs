using dbot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dbot.CommandModules
{
    [Group("info")]
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
        public async Task Default(string movieName)
        {
            var movie = await _omdbService.GetMovieByTitle(movieName);
            Console.WriteLine($"Retrieved movie info for \"{movieName}\"; id={movie.imdbID}");
            await ReplyAsync(movie.ToString());
        }

        [Command]
        [Name("By Year")]
        [Summary("Searches for information of a movie by name and year")]
        [Remarks("Usage: !info <movie name> <year>")]
        public async Task Default(string movieName, int year)
        {
            var movie = await _omdbService.GetMovieByTitleYear(movieName, year);
            Console.WriteLine($"Retrieved movie info for \"{movieName}\", {year}; id={movie.imdbID}");
            await ReplyAsync(movie.ToString());
        }
    }
}
