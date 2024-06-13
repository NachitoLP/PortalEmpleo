using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PortalEmpleo.Models;
using System.Security.Claims;
using System.Data;
using PortalEmpleo.Utils;

namespace PortalEmpleo.Controllers
{
	public class RegisterController : Controller
	{
		private string cstring = "FX-NB-001\\MSSQLSERVER02";
		private string csdb = "PortalEmpleo";
		public IActionResult Index()
		{
			SqlConnectionStringBuilder connectionString = new();
			connectionString.DataSource = cstring;
			connectionString.InitialCatalog = csdb;
			connectionString.IntegratedSecurity = true;

			var cs = connectionString.ConnectionString;

			var viewModel = new RegisterViewModel();

			viewModel.Roles = UserUtils2.BringRoles();

			return View(viewModel);
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

			var password = formData["user_password"];

            var hashedPassword = PasswordHasher.HashPassword(password);
            var parts = hashedPassword.Split(':');
            var passwordHash = parts[1];
            var passwordSalt = parts[0];

            var user = new User
			{
				UserName = formData["user_name"],
				UserSurname = formData["user_surname"],
				UserEmail = formData["user_email"],
				UserPassword = passwordHash,
				UserPasswordSalt = passwordSalt,
				UserBirthDate = DateTime.Parse(formData["user_birth_date"]),
				RoleDescription = selectedRole
			};

			if (user.UserBirthDate > DateTime.Now)
			{
				var viewModel = new RegisterViewModel();

				viewModel.Roles = UserUtils2.BringRoles();
				viewModel.FutureDateError = "La fecha de nacimiento no puede ser posterior a la fecha actual.";

				return View(viewModel);
			}

			user.UserAge = UserUtils.CalculateAge(user.UserBirthDate);

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Users(user_name, user_surname, user_email, user_password, user_birth_date, role_description, user_age, user_password_salt) VALUES(@UserName, @UserSurname, @UserEmail, @UserPassword, @UserBirthDate, @RoleDescription, @UserAge, @UserPasswordSalt)", connection);
				cmd.Parameters.AddWithValue("@UserName", user.UserName);
				cmd.Parameters.AddWithValue("@UserSurname", user.UserSurname);
				cmd.Parameters.AddWithValue("@UserEmail", user.UserEmail);
				cmd.Parameters.AddWithValue("@UserPassword", user.UserPassword);
				cmd.Parameters.AddWithValue("@UserBirthDate", user.UserBirthDate);
				cmd.Parameters.AddWithValue("@RoleDescription", user.RoleDescription);
				cmd.Parameters.AddWithValue("@UserAge", user.UserAge);
                cmd.Parameters.AddWithValue("@UserPasswordSalt", user.UserPasswordSalt);
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
