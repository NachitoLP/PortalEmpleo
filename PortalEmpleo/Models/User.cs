using Microsoft.AspNetCore.Mvc;

namespace PortalEmpleo.Models
{
	public class User
	{
		public int id_user { get; set; }
		public string name { get; set; }
		public string surname { get; set; }
		public string email { get; set; }
		public string password { get; set; }
		public DateTime birth_date { get; set; }
		public string role { get; set; }
		public string profile_img { get; set; }
		public int age { get; set; }

		public User()
		{
			age = CalculateAge(birth_date);
		}

		static int CalculateAge(DateTime birth_date)
		{
			DateTime actualDate = DateTime.Today;
			int age = actualDate.Year - birth_date.Year;

			if (birth_date > actualDate.AddYears(-age))
				age--;
			return age;
		}
	}
}
