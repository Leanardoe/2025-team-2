using OpenAI;
using OpenAI.Chat;
using ResumeSystem.Models.Database;
using System.Text;

namespace ResumeSystem.Models
{
    public class AIProcess
    {
        public Resume Resume;

        public static string ProcessResume()
        {
            //use OpenAI to receive a string using https://www.nuget.org/packages/OpenAI-DotNet/
            return "";
        }

        public Candidate CreateCandidate(string str)
        {
            //Take the formatted response from ProcessResume, extract a candidate name, and create a new candidate or add skills to an existing one. 
            return new Candidate();
        }

        public static async Task<Response> ProcessResumeAsync(IFormFile resumeFile, string prompt, OpenAIClient client)
        {
            if (resumeFile == null || resumeFile.Length == 0)
                return new Response("❌ Please upload a valid .txt resume file.",false);

            string resumeText;
            try
            {
                using var reader = new StreamReader(resumeFile.OpenReadStream(), Encoding.UTF8);
                resumeText = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(resumeText))
                    return new Response("❌ The uploaded file appears to be empty.",false);
            }
            catch (Exception readEx)
            {
                return new Response($"❌ Failed to read uploaded file: {readEx.Message}",false);
            }

            string fullPrompt = $"{prompt}\n\n{resumeText}";

            try
            {
                var chatRequest = new ChatRequest(
                    new[]
                    {
                        new Message(Role.System, "You are a resume skill extractor."),
                        new Message(Role.User, fullPrompt)
                    },
                    model: "gpt-3.5-turbo",
                    temperature: 0.4,
                    maxTokens: 500
                );

                var response = await client.ChatEndpoint.GetCompletionAsync(chatRequest);
                var resultContent = response?.FirstChoice?.Message?.Content?.ToString();

                return string.IsNullOrWhiteSpace(resultContent)
                    ? new Response("⚠️ OpenAI returned no content. The response was empty.",false)
                    : new Response(resultContent,true);
            }
            catch (HttpRequestException ex)
            {
                return new Response($"❌ OpenAI HTTP error: {ex.Message}",false);
            }
            catch (Exception ex)
            {
                return new Response($"❌ Unexpected error: {ex.Message}",false);
            }
        }
    }
}
