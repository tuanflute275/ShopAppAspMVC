using Newtonsoft.Json;

namespace ShopApp.Models.reCaptcha
{
    public class RecaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public string ChallengeTs { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
