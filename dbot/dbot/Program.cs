using System;

namespace dbot
{
    class Program
    {
        static void Main(string[] args)
        {
            var discordToken = TokenManager.getToken(TokenKey.DiscordToken);
            var omdbToken = TokenManager.getToken(TokenKey.OMDBToken);
            Console.WriteLine($"Hello World! {omdbToken} {discordToken}");
        }
    }
}
