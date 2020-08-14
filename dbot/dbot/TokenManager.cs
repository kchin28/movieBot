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

    class TokenManager
    {
        private static readonly Dictionary<TokenKey, string> tokens = new Dictionary<TokenKey, string>()
        {
            { TokenKey.DiscordToken, "discordToken" },
            { TokenKey.OMDBToken, "omdbToken" },
            { TokenKey.NominationsFile, "nominationsFile" },
            { TokenKey.VotesFile, "votesFile" }
        };

        private readonly string _secretsUri;

        public TokenManager(string secretsUri)
        {
            _secretsUri = secretsUri;
        }

        public TokenManager()
        {
        #if DEBUG
            _secretsUri = "debugsecrets.json";
        #else
            _secretsUri = "secrets.json";
        #endif
        }

        public string GetToken(TokenKey tok)
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
