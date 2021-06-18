using ITF.SDK.DavisCup.Interfaces;
using ITF.SDK.DavisCup.Models;
using RestSharp;
using System.Collections.Generic;

namespace ITF.SDK.DavisCup.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IConfiguration _config;

        public PlayerService(IConfiguration config)
        {
            _config = config;
        }

        public PlayerModel GetPlayer(int playerId, string token = null)
        {
            var client = new RestClient(_config.ApiUrl);
            var request = new RestRequest("player/dc/{id}", Method.GET);
            request.AddUrlSegment("id", playerId.ToString());

            if (token != null)
            {
                request.AddHeader(Configuration.TokenHeader, token);
            }
            var response = client.Execute<List<PlayerModel>>(request);
            if (response.Data != null && response.Data.Count > 0)
            {
                return response.Data[0];
            }
            return null;
        }
    }
}
