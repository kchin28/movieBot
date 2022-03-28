
using dbot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace dbot.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MovieBotContext context)
        {
            context.Database.Migrate();

           //none of the tables need to be seeded at initial creation / before a weekly session
           //tables can/should be seeded if db was lost during an open vote/open nomination session -- could seed from autoDictionary xml backup or from creeping discord channel?
           //eventually db will hold the running list of nominations and winners. These two tables holds more valuable info, might be able to seed from creeping discord channels? 
        }
    }
}