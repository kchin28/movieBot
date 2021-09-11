﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using dbot.Services;
using dbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dbot.Data;

namespace dbot.CommandModules
{
    [Group("Nominate")]
    [Summary("Commands for nominating movies")]
    public class NominationsModule : ModuleBase
    {
        private NominationsService _nominationsService;
        private  OmdbService _omdbService;
        private  VotingService _votingService;
        

        public NominationsModule(NominationsService ns, OmdbService os, VotingService vs)
        {
            _nominationsService = ns;
            _omdbService = os;
            _votingService = vs;
            
        }

        [Command]
        [Name("By Name")]
        [Summary("Nominate a movie by name")]
        [Remarks("Usage: !nominate <movie name>")]
        [Priority(1)]
        public async Task AddNominationASync([Remainder]string name)
        {
            try {
            Console.WriteLine($"Got nomination request for \"{name}\"");
            if (!_votingService.VotingOpen())
            {
                var movie = await _omdbService.GetMovieByTitle(name);

                if (movie.Title.Equals(null))
                {
                    Console.WriteLine($"Failed to find nominated movie \"{name}\"");
                    await ReplyAsync("Could not find this movie.");
                }
                else 
                {
                    if(!_nominationsService.IsNominated(movie.ImdbId))
                    {
                        Console.WriteLine($"Adding nominated movie \"{name}\"");
                        await ReplyAsync(movie.ToString());

                        // If this isnt the right one, specify the year and change the nomination
                        _nominationsService.AddNomination(Context.User, movie.Title, movie.ImdbId);
                        await ReplyAsync("Thanks for nominating!");
                    }
                    else
                    {
                        Console.WriteLine($"Attempted to nominate duplicate movie \"{name}\"");
                        await ReplyAsync($"{movie.Title} is already nominated!");
                    }
                }
            }
            else
            {
                await ReplyAsync("Cannot nominate during open voting session");
            }     
            } catch (Exception ex) {Console.WriteLine("Could not add Nomination: "+ ex.Message);}

        }

        [Command("year")] 
        [Name("By Name and Year")]
        [Summary("Nominate a movie by name and year")]
        [Remarks("Usage: !nominate year \"<movie name>\" <year>")]
        [Priority(2)]
        public async Task AddNominationWithYearASync(string name, int year)
        {
            Console.WriteLine($"Got nomination request for \"{name}\", {year}");
            if (!_votingService.VotingOpen())
            {

                var movie = await _omdbService.GetMovieByTitleYear(name, year);

                if (movie.Title.Equals(null))
                {
                    Console.WriteLine($"Failed to find nominated movie \"{name}\", {year}");
                    await ReplyAsync("Could not find this movie.");
                }
                else
                {
                    if(!_nominationsService.IsNominated(movie.ImdbId))
                    {
                        Console.WriteLine($"Adding nominated movie \"{name}\"");
                        await ReplyAsync(movie.ToString());

                        // If this isnt the right one, specify the year and change the nomination obj
                        _nominationsService.AddNomination(Context.User, movie.Title, movie.ImdbId);
                        await ReplyAsync("Thanks for nominating!");
                    }
                    else
                    {
                        Console.WriteLine($"Attempted to nominate duplicate movie \"{name}\"");
                        await ReplyAsync($"{movie.Title} is already nominated!");
                    }
                }
            }
            else
            {
                await ReplyAsync("Cannot nominate during open voting session");
            }
        }

        [Command("id")]
        [Name("By IMDB Id")]
        [Summary("Nominate a movie by IMDB id")]
        [Remarks("Usage: !nominate id <id>")]
        [Priority(3)]
        public async Task NominateById(string id)
        {
            Console.WriteLine($"Got nomination request for {id}");
            if (!_votingService.VotingOpen())
            {
                var mov = await _omdbService.GetMovieById(id);

                if (mov.Title.Equals(null))
                {
                    Console.WriteLine($"Failed to find nominated movie {id}");
                    await ReplyAsync("Could not find this movie.");
                }
                else
                {
                    if(!_nominationsService.IsNominated(mov.ImdbId))
                    {
                        Console.WriteLine($"Adding nominated movie \"{id}\"");
                        await ReplyAsync(mov.ToString());

                        // If this isnt the right one, specify the year and change the nomination
                        _nominationsService.AddNomination(Context.User, mov.Title, mov.ImdbId);
                        await ReplyAsync("Thanks for nominating!");
                    }
                    else
                    {
                        Console.WriteLine($"Attempted to nominate duplicate movie \"{id}\"");
                        await ReplyAsync($"{mov.Title} is already nominated!");
                    }
                }
            }
            else
            {
                await ReplyAsync("Cannot nominate during open voting session");
            }
        }

        [Command("View")]
        [Summary("Prints a list of current nominations")]
        [Remarks("Usage: !nominate view")]
        [Priority(3)]
        public async Task ViewNominationsAsync()
        {
            Console.WriteLine("Got request to display nominations");
            string result = _nominationsService.ViewNominations();
            await ReplyAsync(result);

        }

        [Command("Delete")]
        [Summary("Delete your nomination")]
        [Remarks("Usage: !nominate delete")]
        [Priority(3)]
        public async Task DeleteNominationForUser() 
        {
            Nomination nomination;
            if(_nominationsService.UserHasNomination(Context.User, out nomination))
            {
                _nominationsService.DeleteNominationForUser(Context.User);
                Console.WriteLine($"Deleted {nomination.Name} from nominations (nominated by {Context.User})");
                await ReplyAsync($"Deleted {nomination.Name} from nominations!");
            }
            else
            {
                Console.WriteLine($"Got delete nomination request from {Context.User}, user has no nomination");
                await ReplyAsync("You do not have an open nomination!");
            }
        }

    }
}   
