namespace EmailService.API.DTOs
{
    public record EmailRequestDto(string Recipient, string Subject, string MessageBody);
}
