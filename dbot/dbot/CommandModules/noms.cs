﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using dbot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dbot.CommandModules
{
    [Group("nominate")]
    public class noms : ModuleBase
    {
        private readonly NominationsService _nominationsService;
        private readonly OmdbService _omdbService;
        private readonly VotingService _votingService;

        public noms(NominationsService ns, OmdbService os, VotingService vs)
        {
            _nominationsService = ns;
            _omdbService = os;
            _votingService = vs;
        }

        [Command]
        public async Task addNominationASync([Remainder]string name)
        {
            if (!_votingService.votingOpen())
            {
                var movie = await _omdbService.GetMovieByTitle(name);

                if (movie.Title.Equals(null))
                {
                    await ReplyAsync("Could not find this movie.");
                }
                else {
                    await ReplyAsync(movie.ToString());

                    //if this isnt the right one, specify the year and change the nomination
                    _nominationsService.addNom(Context.User, movie.Title, movie.imdbID);
                    await ReplyAsync("Thanks for nominating!");
                }
            }
            else {
                await ReplyAsync("Cannot nominate during open voting session");
            }      
        }

        [Command] 
        public async Task addNominationWithYearASync(string name, int year) {
            if (!_votingService.votingOpen())
            {

                var movie = await _omdbService.GetMovieByTitleYear(name, year);

                if (movie.Title.Equals(null))
                {
                    await ReplyAsync("Could not find this movie.");
                }
                else
                {
                    await ReplyAsync(movie.ToString());

                    //if this isnt the right one, specify the year and change the nomination obj
                    _nominationsService.addNom(Context.User, movie.Title, movie.imdbID);
                    await ReplyAsync("Thanks for nominating!");
                }
            }
            else {
                await ReplyAsync("Cannot nominate during open voting session");
            }
        }

        [Command("id")]
        [Priority(1)]
        public async Task nomByID(string id) {
            if (!_votingService.votingOpen())
            {
                var mov = await _omdbService.GetItemByID(id);

                if (mov.Title.Equals(null))
                {
                    await ReplyAsync("Could not find this movie.");
                }
                else
                {
                    await ReplyAsync(mov.ToString());

                    //if this isnt the right one, specify the year and change the nomination
                    _nominationsService.addNom(Context.User, mov.Title, mov.imdbID);
                    await ReplyAsync("Thanks for nominating!");
                }
            }
            else {
                await ReplyAsync("cannot nominate during open voting session");
            }
        }

        [Command("view")]
        [Priority(2)]
        public async Task viewNominationsAsynch() {
            string result = _nominationsService.viewNominations();
            await ReplyAsync(result);

        }

    }
}   
