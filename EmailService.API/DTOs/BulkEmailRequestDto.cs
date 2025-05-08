namespace EmailService.API.DTOs
{
    public record BulkEmailRequestDto(List<string> Recipients, string Subject, string MessageBody)
    {
        public BulkEmailRequestDto() : this(new(), string.Empty, string.Empty) { }
    }
}
