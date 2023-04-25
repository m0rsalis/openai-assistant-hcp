namespace NetOpenApi.Api.Models
{
    public class ChatResponse
    {
        public bool RequestFullyParsed { get; set; }
        public string FollowupMessage { get; set; }
        public string? ParsedModel { get; set; }
        public string SessionId { get; set; }
    }
}
