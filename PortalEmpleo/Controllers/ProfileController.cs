using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

using PortalEmpleo.Models;
using PortalEmpleo.Utils;

namespace PortalEmpleo.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
        public IActionResult Index()
		{
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

            var user = UserUtils2.getUser(userEmail);

            var userId = user.UserId;

			try
			{
                UserProfileViewModel viewModel = new UserProfileViewModel
                {
                    UserName = user.UserName,
                    UserSurname = user.UserSurname,
                    UserEmail = user.UserEmail,
                    UserBirthDate = user.UserBirthDate,
                    UserProfileImg = user.UserProfileImg,
                    UserTitle = user.UserTitle,
                    UserDescription = user.UserDescription,
                    RoleDescription = user.RoleDescription
                };

                List<Post> userPosts = PostUtils.GetPostsByUser(userId);
                List<Role> userRoles = UserUtils2.BringRoles();

                viewModel.Roles = userRoles;
                viewModel.Posts = userPosts;

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
                    UserTitle = user.UserTitle,
                    UserDescription = user.UserDescription,
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
        public IActionResult DeletePost(int id)
        {
            try
            {
                var cs = DBHelper.GetConnectionString();

                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    string sqlQuery = "DELETE FROM dbo.Posts WHERE post_id = @PostId";

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    cmd.Parameters.AddWithValue("@PostId", id);

                    cmd.ExecuteNonQuery();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    connection.Close();

                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones si es necesario
                Console.WriteLine("Error al eliminar el post:", ex.Message);
                return StatusCode(500); // Devolver un código de error HTTP 500 en caso de error
            }
        }

        [HttpPost]
        public async Task<IActionResult> ModifyProfile(UserProfileViewModel model)
        {
            var cs = DBHelper.GetConnectionString();
            try
            {
                var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

                var user = UserUtils2.getUser(userEmail);
                // Verifica si el usuario ha proporcionado una nueva imagen de perfil
                string sqlQuery;
                if (model.NewProfileImage != null)
                {
                    // Si hay una nueva imagen de perfil, actualiza la consulta SQL para incluir la imagen de perfil
                    sqlQuery = "UPDATE dbo.Users SET user_name = @NewUserName, user_surname = @NewUserSurname, user_birth_date = @NewUserBD, user_profile_img = @NewUserPI, role_description = @NewUserRole, user_title_description = @UserTitle, user_about_me = @UserDescription WHERE user_email = @UserEmail";
                }
                else
                {
                    // Si no hay una nueva imagen de perfil, actualiza la consulta SQL para omitir la imagen de perfil
                    sqlQuery = "UPDATE dbo.Users SET user_name = @NewUserName, user_surname = @NewUserSurname, user_birth_date = @NewUserBD, role_description = @NewUserRole, user_title_description = @UserTitle, user_about_me = @UserDescription WHERE user_email = @UserEmail";
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
                        UserTitle = user.UserTitle,
                        UserDescription = user.UserDescription,
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

                    if (model.UserTitle == null) {
                        cmd.Parameters.AddWithValue("@UserTitle", "");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@UserTitle", model.UserTitle);
                    }

                    if (model.UserDescription == null) {
                        cmd.Parameters.AddWithValue("@UserDescription", "");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@UserDescription", model.UserDescription);
                    }

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
