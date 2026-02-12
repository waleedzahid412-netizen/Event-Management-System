using Event_Management_System.Configuration;
using Event_Management_System.DTOs;
using Event_Management_System.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace Event_Management_System.Services.Implementations
{
    public class RecommendationService :IRecommendationService
    { 
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public RecommendationService(
            HttpClient httpClient,
            IOptions<RecommendationApiSettings> options)
        {
            _httpClient = httpClient;
            _baseUrl = options.Value.BaseUrl.TrimEnd('/');
        }
        public async Task<RecommendationResponseDTO> GetRecommendationsAsync(
     int eventId,
     int topN = 5)
        {
            var request = new RecommendationRequestDTO
            {
                EventId = eventId,
                TopN = topN
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_baseUrl}/recommend",
                request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(
                    $"Recommendation API failed ({response.StatusCode}): {error}");
            }

            var result = await response.Content
                .ReadFromJsonAsync<RecommendationResponseDTO>();

            return result!;
        }
    }
}
