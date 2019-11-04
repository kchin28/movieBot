using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using dbot.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace dbot.Services
{
    public class OmdbService
    {
        private string _omdbToken;
        private string _baseUri;
        private string _posterBaseUri;

        public OmdbService(string omdbToken)
        {
            _omdbToken = omdbToken;
            _baseUri = $"http://www.omdbapi.com/?apikey={_omdbToken}&";
            _posterBaseUri = $"http://www.img.omdbapi.com/?apikey={_omdbToken}&";
        }

        private async Task<T> Request<T>(string query)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"{_baseUri}{query}").ConfigureAwait(false);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return (T)JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings());
                }
                else
                {
                    return default(T);
                }
            }
        }
        
        public async Task<Movie> GetMovieByTitle(string title)
        {
            var movie = await Request<Movie>($"t={title}").ConfigureAwait(false);
            return movie;
        }

        public async Task<Movie> GetMovieByTitleYear(string title, int year)
        {
            var movie = await Request<Movie>($"t={title}&y={year}").ConfigureAwait(false);
            return movie;
        }

        public async Task<Movie> GetMovieById(string id)
        {
            var movie = await Request<Movie>($"i={id}").ConfigureAwait(false);
            return movie;
        }
    }
}
