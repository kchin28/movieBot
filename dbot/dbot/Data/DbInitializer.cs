
using dbot.Models;
using System;
using System.Linq;

namespace dbot.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MovieBotContext context)
        {
            context.Database.EnsureCreated();

            if (context.WeeklyNominations.Any())
            {
                return;   // DB has been seeded
            }
            else 
            {
                
            }
        }
    }
}