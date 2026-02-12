using System.Text.Json.Serialization;

namespace Event_Management_System.DTOs
{
    public class RecommendationRequestDTO
    {
        [JsonPropertyName("event_id")]
        public int EventId { get; set; }
        [JsonPropertyName("top_n")]
        public int TopN { get; set; } = 5;
    }
}
