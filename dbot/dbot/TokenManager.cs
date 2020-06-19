using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;

namespace dbot
{
    enum TokenKey
    {
        DiscordToken,
        OMDBToken,
        NominationsFile,
        VotesFile
    }

    static class TokenManager
    {
        private static readonly Dictionary<TokenKey, string> tokens = new Dictionary<TokenKey, string>()
        {
            { TokenKey.DiscordToken, "discordToken" },
            { TokenKey.OMDBToken, "omdbToken" },
            { TokenKey.NominationsFile, "nominationsFile" },
            { TokenKey.VotesFile, "votesFile" }
        };
    #if DEBUG
        private static readonly string _secretsUri = "debugsecrets.json";
    #else
        private static readonly string _secretsUri = "secrets.json";
    #endif
        
        public static string GetToken(TokenKey tok)
        {
            string tokenKey = tokens[tok];

            using (var fs = System.IO.File.OpenRead(_secretsUri))
            using (var sReader = new System.IO.StreamReader(fs))
            {
                var jobj = JObject.Parse(sReader.ReadToEnd());
                return jobj.SelectToken(tokenKey).Value<String>();
            }
        }
    }
}