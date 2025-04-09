namespace ResumeSystem.Models
{
    public class Response
    {
        public string Text { get; set; }

        public bool Correct { get; set; }

        public Response(string text, bool error)
        {
            Text = text;
            Correct = error;
        }
    }
}
