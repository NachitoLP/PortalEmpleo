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

        public IActionResult Index()
		{
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

            var user = UserUtils2.getUser(userEmail);


			try
			{
                UserProfileViewModel viewModel = new UserProfileViewModel
                {
                    UserName = user.UserName,
                    UserSurname = user.UserSurname,
                    UserEmail = user.UserEmail,
                    UserBirthDate = user.UserBirthDate,
                    UserProfileImg = user.UserProfileImg,
                    RoleDescription = user.RoleDescription
                };

                List<Role> userRoles = UserUtils2.BringRoles();
                viewModel.Roles = userRoles;

                return View(viewModel);
                
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);

				throw new Exception("Error al obtener los datos del usuario.", ex);
			}
		}

        public IActionResult ModifyProfile() {
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

            var user = UserUtils2.getUser(userEmail);

            try
            {
                UserProfileViewModel viewModel = new UserProfileViewModel
                {
                    UserName = user.UserName,
                    UserSurname = user.UserSurname,
                    UserEmail = user.UserEmail,
                    UserBirthDate = user.UserBirthDate,
                    UserProfileImg = user.UserProfileImg,
                    RoleDescription = user.RoleDescription
                };

                List<Role> userRoles = UserUtils2.BringRoles();
                viewModel.Roles = userRoles;

                return View(viewModel);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw new Exception("Error al obtener los datos del usuario.", ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ModifyProfile(UserProfileViewModel model)
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = cstring;
            connectionString.InitialCatalog = csdb;
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;
            try
            {
                var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

                var user = UserUtils2.getUser(userEmail);
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

                if (model.UserBirthDate > DateTime.Now)
                {

                    UserProfileViewModel viewModel = new UserProfileViewModel
                    {
                        UserName = user.UserName,
                        UserSurname = user.UserSurname,
                        UserEmail = user.UserEmail,
                        UserBirthDate = user.UserBirthDate,
                        UserProfileImg = user.UserProfileImg,
                        RoleDescription = user.RoleDescription
                    };

                    List<Role> userRoles = UserUtils2.BringRoles();
                    viewModel.Roles = userRoles;
                    viewModel.FutureDateError = "La fecha de nacimiento no puede ser posterior a la fecha actual.";

                    return View(viewModel);
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
                model.Roles = UserUtils2.BringRoles();

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el perfil del usuario: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al actualizar el perfil del usuario. Por favor, inténtelo de nuevo.");
                model.Roles = UserUtils2.BringRoles();

                return View(model);
            }
        }

    }
}
