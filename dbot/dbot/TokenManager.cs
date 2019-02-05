using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace dbot
{
    enum TokenKey
    {
        DiscordToken,
        OMDBToken
    }

    static class TokenManager
    {
        private static readonly string _discordToken = "discordToken";
        private static readonly string _omdbToken = "omdbToken";
    #if DEBUG
        private static readonly string _secretsUri = "debugsecrets.json";
    #else
        private static readonly string _secretsUri = "secrets.json";
    #endif
        
        public static string getToken(TokenKey tok)
        {
            string token = null;
            string tokenKey = null;
            switch (tok)
            {
                case TokenKey.DiscordToken:
                    tokenKey = _discordToken;
                    break;
                case TokenKey.OMDBToken:
                    tokenKey = _omdbToken;
                    break;
            }
            using (var fs = System.IO.File.OpenRead(_secretsUri))
            using (var sReader = new System.IO.StreamReader(fs))
            {
                var jobj = JObject.Parse(sReader.ReadToEnd());
                token = jobj.SelectToken(tokenKey).Value<String>();
            }
            return token;
        }
    }
}