using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PortalEmpleo.Models;

namespace PortalEmpleo.Controllers
{
	public class LoginController : Controller
	{
		private string cstring = "DESKTOP-9NLV2TR\\MSSQLSERVER10";
		private string csdb = "PortalEmpleo";

		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Index(string email, string password)
		{
			bool checkUser = false;
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
				cmd.CommandText = $"[LoginUser] @email = '{email}', @password = '{password}'";

				var reader = cmd.ExecuteReader();
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						checkUser = true;

						//user.id_user = (int)reader["id_user"];
						//user.name = (string)reader["name"];
						//user.surname = (string)reader["surname"];
						//user.email = (string)reader["email"];
						//user.birth_date = (DateTime)reader["birth_date"];
						//user.age = (int)reader["age"];
						//user.role = (string)reader["role"];
					}
					reader.Close();
				}
			}
			string jsonResponse = "";

			if(checkUser)
			{
				
				return RedirectToAction("Home", "Index");
			}
			else
			{
				jsonResponse = "{\"status\": 500, \"data\": \"ERROR, el usuario no existe en la BD\"}";
			}

			return Json(jsonResponse);
		}
	}
}
