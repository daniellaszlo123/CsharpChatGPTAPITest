using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetEnv;

namespace ChatGPTAPITest
{
    public class ChatGPTAPIHandler
    {
        
        private readonly string _apiKey;
        private readonly string _apiUrl;
        
        public ChatGPTAPIHandler()
        {
            Env.Load();

            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
            _apiUrl = Environment.GetEnvironmentVariable("OPENAI_API_URL") ?? "";
        }

        public async Task<string> MakeRequest(string prompt, string requestContent)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo", // teszteléshez
                messages = new[]
                {
                new { role = "system", content = prompt },
                new { role = "user", content = requestContent }
            },
                temperature = 0.7
                /* 
                 * temperature értéke 0.0 és 2.0 között lehet 
                 * Alacsonyabb érték: konzervatívabb, kiszámíthatóbb válaszok
                 * Magasabb érték: kreatívabb, változatosabb válaszok
                 * temperature = 0.2: nagyon precíz, „robotikusabb” válasz, jó pl. tényekhez, kódhoz
                 * temperature = 0.7: egyensúlyban van a pontosság és kreativitás – ez az alapértelmezett érték sok esetben
                 * temperature = 1.0+: nagyon kreatív, jó pl. történetíráshoz, ötleteléshez – de néha „kitalál” dolgokat
                 */
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_apiUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            var message = string.Empty;

            if (response.IsSuccessStatusCode)
            {
                var jsonDoc = JsonDocument.Parse(responseString);
                message = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            else
            {
                Console.WriteLine("Hiba: " + response.StatusCode);
                Console.WriteLine(responseString);
            }

            return message ?? "";
        }
    }
}
