using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace OAuthWithGithub.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        public string Message { get; set; }

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string code, string state)
        {
            var httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", "Ov23li9LNCpGfDDQWZFM" },
                { "client_secret", "ccc6a117e6fc8f103aa5fdecf89eb3a95019b17e" },
                { "code",code }
            });

            var accessTokenResponse = httpClient.PostAsync("https://github.com/login/oauth/access_token?", content).Result;

            var token = accessTokenResponse.Content.ReadAsStringAsync().Result;
            var response = token.Split('&').Select(x =>
                                                x.Split('=')).ToDictionary(key => key[0],
                                                value => value[1]);

            var accessToken = response["access_token"];

            var tokenRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");

            tokenRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            tokenRequest.Headers.Add("User-Agent", "OAuthWithGithub");

            var responseOfGivenAccessToken = httpClient.SendAsync(tokenRequest).Result;

            var responseOfGivenAccessTokenInString = responseOfGivenAccessToken.Content.ReadAsStringAsync().Result;

            if (responseOfGivenAccessToken.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Message = "Authorized successfully";

                var userJson = JObject.Parse(responseOfGivenAccessTokenInString);
                var userName = (string)userJson["login"];

                Message = $"Authorized successfully. User: {userName}";
            }
            else
            {
                Message = "Authorized failed";
            }
        }
    }
}