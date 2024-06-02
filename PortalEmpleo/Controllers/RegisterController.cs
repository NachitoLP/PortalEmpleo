using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PortalEmpleo.Models;
using System.Security.Claims;
using System.Data;

namespace PortalEmpleo.Controllers
{
	public class RegisterController : Controller
	{
		private string cstring = "DESKTOP-9NLV2TR\\MSSQLSERVER10";
		private string csdb = "PortalEmpleoEFC";
		public IActionResult Index()
		{
			SqlConnectionStringBuilder connectionString = new();
			connectionString.DataSource = cstring;
			connectionString.InitialCatalog = csdb;
			connectionString.IntegratedSecurity = true;

			var cs = connectionString.ConnectionString;
			List<Role> roles = new List<Role>();

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();

				SqlCommand cmd = connection.CreateCommand();
				cmd.CommandText = $"SELECT * FROM dbo.Role";

				var reader = cmd.ExecuteReader();
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						Role role = new Role
						{
							RoleDescription = (string)reader["role_description"]
						};
						roles.Add(role);
					}
					reader.Close();
				}
			}
			return View(roles);
		}

		[HttpPost]
		public async Task<IActionResult> Index(string selectedRole, IFormCollection formData)
		{
			SqlConnectionStringBuilder connectionString = new();
			connectionString.DataSource = cstring;
			connectionString.InitialCatalog = csdb;
			connectionString.IntegratedSecurity = true;

			var cs = connectionString.ConnectionString;

			bool emailExists;
			var checkEmail = formData["user_email"].ToString();

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();
				SqlCommand checkEmailCmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE user_email = @UserEmail", connection);
				checkEmailCmd.Parameters.AddWithValue("@UserEmail", checkEmail);
				int emailCount = (int)checkEmailCmd.ExecuteScalar();
				emailExists = emailCount > 0;
			}

			if (emailExists)
			{
				return BadRequest("El email ya está registrado en la base de datos.");
			}


			var user = new User
			{
				UserName = formData["user_name"],
				UserSurname = formData["user_surname"],
				UserEmail = formData["user_email"],
				UserPassword = formData["user_password"],
				UserBirthDate = DateTime.Parse(formData["user_birth_date"]),
				RoleDescription = selectedRole
			};

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Users(user_name, user_surname, user_email, user_password, user_birth_date, role_description) VALUES(@UserName, @UserSurname, @UserEmail, @UserPassword, @UserBirthDate, @RoleDescription)", connection);
				cmd.Parameters.AddWithValue("@UserName", user.UserName);
				cmd.Parameters.AddWithValue("@UserSurname", user.UserSurname);
				cmd.Parameters.AddWithValue("@UserEmail", user.UserEmail);
				cmd.Parameters.AddWithValue("@UserPassword", user.UserPassword);
				cmd.Parameters.AddWithValue("@UserBirthDate", user.UserBirthDate);
				cmd.Parameters.AddWithValue("@RoleDescription", user.RoleDescription);
				cmd.ExecuteNonQuery();

				connection.Close();
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim("Correo", user.UserEmail),
				new Claim(ClaimTypes.Role, user.RoleDescription)
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

			return RedirectToAction("Index", "Home");
		}
	}
}
