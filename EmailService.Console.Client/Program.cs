using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static readonly HttpClient client = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7109/api/")
    };

    static async Task Main(string[] args)
    {
        Console.WriteLine("Email Sender Console Application");

        try
        {
            await RunEmailSender();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
        }
        finally
        {
            client.Dispose();
        }
    }

    static async Task RunEmailSender()
    {
        while (true)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Send single email");
            Console.WriteLine("2. Send bulk email");
            Console.WriteLine("3. Exit");
            Console.Write("Enter option (1-3): ");
            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await SendSingleEmail();
                    break;
                case "2":
                    await SendBulkEmail();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
                    break;
            }
        }
    }

    static async Task SendSingleEmail()
    {
        Console.Write("Enter recipient email: ");
        var recipient = Console.ReadLine();

        Console.Write("Enter subject: ");
        var subject = Console.ReadLine();

        Console.Write("Enter body: ");
        var body = Console.ReadLine();

        try
        {
            var response = await client.PostAsJsonAsync("Email/send-single-email", new
            {
                Recipient = recipient,
                Subject = subject,
                MessageBody = body
            });

            var responseContent = await HandleResponse(response);
            Console.WriteLine($"Single email queued successfully: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to queue single email: {ex.Message}");
        }
    }

    static async Task SendBulkEmail()
    {
        Console.Write("Enter recipient emails (comma-separated): ");
        var recipientsInput = Console.ReadLine();
        var recipients = recipientsInput?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(r => r.Trim())
                                   .ToArray() ?? Array.Empty<string>();

        Console.Write("Enter subject: ");
        var subject = Console.ReadLine();

        Console.Write("Enter body: ");
        var body = Console.ReadLine();

        try
        {
            var response = await client.PostAsJsonAsync("Email/send-bulk-emails", new
            {
                Recipients = recipients,
                Subject = subject,
                MessageBody = body
            });

            var responseContent = await HandleResponse(response);
            Console.WriteLine($"Bulk email queued successfully: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to queue bulk email: {ex.Message}");
        }
    }

    static async Task<string> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream, options);

        if (result.TryGetProperty("message", out JsonElement message))
        {
            return message.GetString() ?? "Email queued successfully (no message provided)";
        }

        return "Email queued successfully (no details provided)";
    }
}