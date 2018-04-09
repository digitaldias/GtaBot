using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GtaBot.Cognitive
{
    public class GtaVisionApi
    {
        private static string API_URI        = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.1/Prediction/c02aa2f4-6a6e-4cea-aa2d-ce9e8a6fbd23/image";
        private static string PREDICTION_KEY = "4a51400ea9504c2b94011244da79bd1b";
        private static HttpClient _httpClient;


        public GtaVisionApi()
        {
            
            if(_httpClient == null)
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("Prediction-Key", PREDICTION_KEY);

            }
        }

        public async Task<VisionResponse> GetPrediction(byte[] imageData)
        {
            var content                 = new ByteArrayContent(imageData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.PostAsync(API_URI, content);
            if(response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<VisionResponse>(text);
            }
            return null;
        }
    }
}