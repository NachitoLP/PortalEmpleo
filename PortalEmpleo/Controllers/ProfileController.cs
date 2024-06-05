﻿using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using PortalEmpleo.Models;
using PortalEmpleo.Utils;
using System.Data;

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
                        string userProfileImg = (reader["user_profile_img"] == DBNull.Value) ? "" : (string)reader["user_profile_img"];
                        user.RoleDescription = (string)reader["role_description"];
					}
					reader.Close();

					return View(user);
				}
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);

				throw new Exception("Error al obtener los datos del usuario.", ex);
			}
		}

        [HttpPost]
        public IActionResult Index(UserProfileViewModel model)
        {
            SqlConnectionStringBuilder connectionString = new();
            connectionString.DataSource = cstring;
            connectionString.InitialCatalog = csdb;
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;

            if (ModelState.IsValid)
            {
				try{
                    var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

                    using (SqlConnection connection = new SqlConnection(cs))
                    {
                        connection.Open();

                        string sqlQuery = "UPDATE dbo.Users SET user_name = @NewUserName, user_surname = @NewUserSurname, user_birth_date = @newUserBD, user_profile_img = @newUserPI, role_description = @newUserRole WHERE user_email = @UserEmail";

                        SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                        cmd.Parameters.AddWithValue("@NewUserName", model.UserName);
                        cmd.Parameters.AddWithValue("@NewUserSurname", model.UserSurname);
                        cmd.Parameters.AddWithValue("@NewUserBD", model.UserBirthDate);
                        cmd.Parameters.AddWithValue("@NewUserPI", model.UserProfileImg);
                        cmd.Parameters.AddWithValue("@NewUserRole", model.RoleDescription);
                        cmd.Parameters.AddWithValue("@UserEmail", userEmail);

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
                    return View(model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar el perfil del usuario: {ex.Message}");
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el perfil del usuario. Por favor, inténtelo de nuevo.");
                    return View(model);
                }
            }
            // Si hay errores de validación, volver a mostrar el formulario con los mensajes de error
            return View(model);
        }
    }
}