using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models.Database;
using System.Text.RegularExpressions;

namespace ResumeSystem.Models
{
	public class FileUpload
	{

		private readonly ResumeContext _context;

		public FileUpload(ResumeContext context)
		{
			_context = context;
		}
		public void ResumeUpload(string filePath, string AIData)
		{
			var resume = new Resume
			{
				RESUME_URL = filePath,
				RESUME_STRING = Resume.ResumeToString(filePath)
			};

			AIData = "{Phil Louis,email@email.com,555-808-9987}|C#,Java,Music,Singing,The News"; //remove this later

			
		}
	}
}
