namespace Tidbeat.DTOs.ChatBeat {
    public class GetRecentMessageDto {
        public string ConversationId { get; set; }
        public int MessageAmount { get; set; }
        public int SkipAmount { get; set; }
    }
}
