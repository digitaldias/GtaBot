using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GtaBot.Cognitive
{
    public class GtaVisionApi
    {
        private static HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _predictionKey;


        public GtaVisionApi(string apiUrl, string predictionKey)
        {
            _apiUrl = apiUrl;
            _predictionKey = predictionKey;

            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("Prediction-Key", _predictionKey);

            }
        }


        public async Task<VisionResponse> GetPrediction(byte[] imageData)
        {
            var content                 = new ByteArrayContent(imageData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            if(response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<VisionResponse>(text);
            }
            return null;
        }
    }
}