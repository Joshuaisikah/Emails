namespace EmailService.API.DTOs
{
    public record BulkEmailResponseDto
    {
        public int TotalRecipients { get; init; }
        public int SuccessfullyQueued { get; init; }
        public List<string> FailedRecipients { get; init; } = new();
    }
}
