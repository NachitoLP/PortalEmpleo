using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using PortalEmpleo.Models;
using PortalEmpleo.Utils;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

namespace PortalEmpleo.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private string cstring = "FX-NB-001\\MSSQLSERVER02";
		private string csdb = "PortalEmpleo";

        private List<string> ObtenerRoles()
        {
            SqlConnectionStringBuilder connectionString = new();
            connectionString.DataSource = cstring;
            connectionString.InitialCatalog = csdb;
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;

            List<string> userRoles = new List<string>();
            using (SqlConnection connectionRole = new SqlConnection(cs))
            {
                connectionRole.Open();

                string sqlQuery = "SELECT role_description FROM dbo.Role";

                SqlCommand cmdRole = new SqlCommand(sqlQuery, connectionRole);
                var readerRole = cmdRole.ExecuteReader();

                while (readerRole.Read())
                {
                    string roleDescription = (string)readerRole["role_description"];
                    userRoles.Add(roleDescription);
                }

                readerRole.Close();
            }
            return userRoles;
        }

        public IActionResult Index()
		{
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

			var user = new UserProfileViewModel();

			SqlConnectionStringBuilder connectionString = new();
			connectionString.DataSource = cstring;
			connectionString.InitialCatalog = csdb;
			connectionString.IntegratedSecurity = true;

			var cs = connectionString.ConnectionString;

			try
			{
				using (SqlConnection connection = new SqlConnection(cs))
				{
					connection.Open();

					SqlCommand cmd = connection.CreateCommand();
					cmd.CommandText = "GetUserByEmail";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@user_email", userEmail);

					var reader = cmd.ExecuteReader();

                    while (reader.Read())
					{
						user.UserName = (string)reader["user_name"];
						user.UserSurname = (string)reader["user_surname"];
						user.UserEmail = (string)reader["user_email"];
						user.UserBirthDate = (DateTime)reader["user_birth_date"];
                        user.RoleDescription = (string)reader["role_description"];
                        if (reader["user_profile_img"] == DBNull.Value)
                        {
                            // Asigna una imagen predeterminada
                            byte[] imagenDefaultBytes = UserUtils2.ObtenerBytesImagenDefault();
                            user.UserProfileImg = imagenDefaultBytes; // Debes definir esta función
                        }
                        else
                        {
                            // Convierte la imagen de la base de datos a un arreglo de bytes
                            user.UserProfileImg = (byte[])reader["user_profile_img"];
                        }
					}
					reader.Close();

                    UserProfileViewModel viewModel = new UserProfileViewModel
                    {
                        UserName = user.UserName,
                        UserSurname = user.UserSurname,
                        UserEmail = user.UserEmail,
                        UserBirthDate = user.UserBirthDate,
                        UserProfileImg = user.UserProfileImg,
                        RoleDescription = user.RoleDescription
                    };

                    List<string> userRoles = ObtenerRoles();
                    viewModel.Roles = userRoles;

                    return View(viewModel);
                }
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);

				throw new Exception("Error al obtener los datos del usuario.", ex);
			}
		}

        [HttpPost]
        public async Task<IActionResult> Index(UserProfileViewModel model)
        {
            SqlConnectionStringBuilder connectionString = new();
            connectionString.DataSource = cstring;
            connectionString.InitialCatalog = csdb;
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;

            try
            {
                var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

                // Verifica si el usuario ha proporcionado una nueva imagen de perfil
                string sqlQuery;
                if (model.NewProfileImage != null)
                {
                    // Si hay una nueva imagen de perfil, actualiza la consulta SQL para incluir la imagen de perfil
                    sqlQuery = "UPDATE dbo.Users SET user_name = @NewUserName, user_surname = @NewUserSurname, user_birth_date = @NewUserBD, user_profile_img = @NewUserPI, role_description = @NewUserRole WHERE user_email = @UserEmail";
                }
                else
                {
                    // Si no hay una nueva imagen de perfil, actualiza la consulta SQL para omitir la imagen de perfil
                    sqlQuery = "UPDATE dbo.Users SET user_name = @NewUserName, user_surname = @NewUserSurname, user_birth_date = @NewUserBD, role_description = @NewUserRole WHERE user_email = @UserEmail";
                }

                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    cmd.Parameters.AddWithValue("@NewUserName", model.UserName);
                    cmd.Parameters.AddWithValue("@NewUserSurname", model.UserSurname);
                    cmd.Parameters.AddWithValue("@NewUserBD", model.UserBirthDate);
                    cmd.Parameters.AddWithValue("@NewUserRole", model.SelectedRole);
                    cmd.Parameters.AddWithValue("@UserEmail", userEmail);

                    // Agrega el parámetro para la imagen de perfil solo si hay una nueva imagen de perfil
                    if (model.NewProfileImage != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.NewProfileImage.CopyToAsync(memoryStream);
                            model.UserProfileImg = memoryStream.ToArray();
                        }
                        cmd.Parameters.AddWithValue("@NewUserPI", model.UserProfileImg);
                    }

                    cmd.ExecuteNonQuery();

                    int newUserAge = UserUtils.CalculateAge(model.UserBirthDate);

                    string sqlAgeUpdateQuery = "UPDATE dbo.Users SET user_age = @NewUserAge WHERE user_email = @UserEmail";
                    SqlCommand ageUpdateCmd = new SqlCommand(sqlAgeUpdateQuery, connection);
                    ageUpdateCmd.Parameters.AddWithValue("@NewUserAge", newUserAge);
                    ageUpdateCmd.Parameters.AddWithValue("@UserEmail", userEmail);

                    ageUpdateCmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error de base de datos al actualizar el perfil del usuario: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al actualizar el perfil del usuario. Por favor, inténtelo de nuevo.");
                model.Roles = ObtenerRoles();

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el perfil del usuario: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al actualizar el perfil del usuario. Por favor, inténtelo de nuevo.");
                model.Roles = ObtenerRoles();

                return View(model);
            }
        }

    }
}
