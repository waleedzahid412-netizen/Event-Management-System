namespace Event_Management_System.DTOs
{
    public class RecommendationResponseDTO
    {
        public int InputEventId { get; set; }
        public List<RecommendedEventDTO> Recommendations { get; set; }
    }
}
