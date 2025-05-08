using EmailService.API.DTOs;
using EmailService.Frontend.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailService.Frontend.Services
{
    public class EmailApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7109/api";

        public EmailApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EmailRecord>> GetEmailsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Email/all");
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var emails = await JsonSerializer.DeserializeAsync<List<EmailRecord>>(stream, options);
                return emails ?? new List<EmailRecord>();
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Request error: {ex.Message}");
                return new List<EmailRecord>();
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"JSON parse error: {ex.Message}");
                return new List<EmailRecord>();
            }
        }

        public async Task<(bool Success, string Message)> SendSingleEmailAsync(string recipient, string subject, string messageBody)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Email/send-single-email", new
                {
                    Recipient = recipient,
                    Subject = subject,
                    MessageBody = messageBody
                });
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<dynamic>(stream, options);

                string message = result.GetProperty("message").GetString();
                return (true, message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to send single email: {ex.Message}");
            }
        }

        public async Task<(bool Success, BulkEmailResponseDto Response)> SendBulkEmailAsync(string[] recipients, string subject, string messageBody)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Email/send-bulk-emails", new
                {
                    Recipients = recipients,
                    Subject = subject,
                    MessageBody = messageBody
                });
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await JsonSerializer.DeserializeAsync<BulkEmailResponseDto>(stream, options);

                return (true, result);
            }
            catch (Exception ex)
            {
                return (false, new BulkEmailResponseDto { FailedRecipients = new List<string>() });
            }
        }
    }
}