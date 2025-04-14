namespace ResumeSystem.Models
{
    public class Response
    {
        public string Text { get; set; }

        public bool Correct { get; set; }

        public string ResumeBody { get; set; } = string.Empty;

        public Response(string text, bool error)
        {
            Text = text;
            Correct = error;
        }

		public Response(string text, bool error, string body)
		{
			Text = text;
			Correct = error;
            ResumeBody = body;
		}
	}
}
