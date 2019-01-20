using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dbot.CommandModules
{
    public class nom1 : ModuleBase
    {

        [Command("nominate"), Summary("nominate a movie with its title")]
        public async Task Nominate(String movName) {
            var omdb = new Omdb();
            var movie = await omdb.GetMovie(movName);

            await
        } 
    }
}   
