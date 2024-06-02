using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PortalEmpleo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PortalEmpleo.Controllers
{
	public class LoginController : Controller
	{
		private string cstring = "DESKTOP-9NLV2TR\\MSSQLSERVER10";
		private string csdb = "PortalEmpleoEFC";

		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Index(string user_email, string user_password)
		{
			var user = new User();

			SqlConnectionStringBuilder connectionString = new();
			connectionString.DataSource = cstring;
			connectionString.InitialCatalog = csdb;
			connectionString.IntegratedSecurity = true;

			var cs = connectionString.ConnectionString;

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();

				SqlCommand cmd = connection.CreateCommand();
				cmd.CommandText = $"[LoginUser] @user_email = '{user_email}', @user_password = '{user_password}'";

				var reader = cmd.ExecuteReader();
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						user.UserName = (string)reader["user_name"];
						user.UserEmail = (string)reader["user_email"];
						user.RoleDescription = (string)reader["role_description"];
					}
					reader.Close();

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
				else
				{
                    string jsonResponse = "{\"status\": 500, \"data\": \"ERROR, el usuario no existe en la BD\"}";
                    return Json(jsonResponse);
                }
			}
		}
	}
}
