﻿using Microsoft.AspNetCore.Mvc;

namespace PortalEmpleo.Models
{
	public class User
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string UserSurname { get; set; }
		public string UserEmail { get; set; }
		public string UserPassword { get; set; }
		public DateTime UserBirthDate { get; set; }
		public string UserProfileImg { get; set; }
		public int UserAge { get; set; }
        public string RoleDescription { get; set; }
        public Role Role { get; set; }

        public List<Skill> Skills { get; set; }

        public List<Company> Companies { get; set; }

        public User()
		{
            UserAge = CalculateAge(UserBirthDate);
		}

        public void AddCompany(Company company)
        {
            Companies.Add(company);
        }

        public int CalculateAge(DateTime user_birth_date)
		{
			DateTime actualDate = DateTime.Today;
			int user_age = actualDate.Year - user_birth_date.Year;

			if (user_birth_date > actualDate.AddYears(-user_age))
                user_age--;
			return user_age;
		}
	}
}
