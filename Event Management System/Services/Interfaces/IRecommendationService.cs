using Event_Management_System.DTOs;

namespace Event_Management_System.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<RecommendationResponseDTO> GetRecommendationsAsync(
    int eventId,
    int topN = 5);
    }
}
