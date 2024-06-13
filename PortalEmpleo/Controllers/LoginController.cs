using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PortalEmpleo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Data;

namespace PortalEmpleo.Controllers
{
	public class LoginController : Controller
	{
		private string cstring = "FX-NB-001\\MSSQLSERVER02";
		private string csdb = "PortalEmpleo";

		public IActionResult Index()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> Index(string user_email, string user_password)
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = cstring;
            connectionString.InitialCatalog = csdb;
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT user_password, user_password_salt, user_name, user_email, role_description FROM dbo.Users WHERE user_email = @user_email";
                cmd.Parameters.AddWithValue("@user_email", user_email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hashedPasswordFromDb = reader.GetString(reader.GetOrdinal("user_password"));
                        string saltFromDb = reader.GetString(reader.GetOrdinal("user_password_salt"));
                        string user_name = reader.GetString(reader.GetOrdinal("user_name"));
                        string roleDescription = reader.GetString(reader.GetOrdinal("role_description"));

                        // Verificar si la contraseña proporcionada coincide con el hash almacenado
                        if (PasswordHasher.VerifyPassword(user_password, $"{saltFromDb}:{hashedPasswordFromDb}"))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user_name),
                                new Claim("Correo", user_email),
                                new Claim(ClaimTypes.Role, roleDescription)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                string jsonResponse = "{\"status\": 500, \"data\": \"ERROR, el usuario no existe en la BD o la contraseña es incorrecta\"}";
                return Json(jsonResponse);
            }
        }

    }
}
