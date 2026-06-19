using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NutriTrackAI.Services
{
    public class NutritionService
    {
        private readonly HttpClient _http;

        public NutritionService(HttpClient http, string userId)
        {
            _http = http;

            // Required for Meal Planner API
            _http.DefaultRequestHeaders.Add("Edamam-Account-User", userId);
            _http.DefaultRequestHeaders.Add("User-Agent", "NutriTrackAI"); // optional
        }

        public async Task<string> TestApiCallAsync()
        {
            string url = "https://api.edamam.com/api/meal-planner/v2/hbhutc/generate?app_id=be3bae0b&app_key=88de42fc223642d6e2ffa048358e368b";

            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP Request failed: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Other error: " + ex.Message);
                return null;
            }
        }
    }
}