using PortalEmpleo.Models;
using System.Data;
using System.Data.SqlClient;

namespace PortalEmpleo.Utils
{
    public static class UserUtils
    {
        public static int CalculateAge(DateTime user_birth_date)
        {
            DateTime actualDate = DateTime.Today;
            int user_age = actualDate.Year - user_birth_date.Year;

            if (user_birth_date > actualDate.AddYears(-user_age))
                user_age--;
            return user_age;
        }
    }

    public static class UserUtils2
    {
		public static User getUser(string userEmail)
		{

            var cs = DBHelper.GetConnectionString();

            var user = new User();

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();

                string sqlQuery = "SELECT * FROM dbo.Users WHERE user_email = @UserEmail";

                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        UserId = (int)reader["user_id"],
                        UserName = (string)reader["user_name"],
                        UserSurname = (string)reader["user_surname"],
                        UserEmail = (string)reader["user_email"],
                        UserBirthDate = (DateTime)reader["user_birth_date"],
                        RoleDescription = (string)reader["role_description"]

                    };

                    if (reader["user_title_description"] != DBNull.Value)
                    {
                        user.UserTitle = (string)reader["user_title_description"];
                    }

                    if (reader["user_about_me"] != DBNull.Value)
                    {
                        user.UserDescription = (string)reader["user_about_me"];
                    }

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
            }
            return user;
        }

		public static byte[] ObtenerBytesImagenDefault()
        {
            string rutaImagenDefault = "wwwroot/Images/perfil_df.jpg"; // Ruta local en tu sistema de archivos
            byte[] bytesImagenDefault = File.ReadAllBytes(rutaImagenDefault);

            return bytesImagenDefault;
        }

		public static List<Role> BringRoles()
		{

            var cs = DBHelper.GetConnectionString();

            List<Role> roles = new List<Role>();

			using (SqlConnection connection = new SqlConnection(cs))
			{
				connection.Open();

				string sqlQuery = "SELECT * FROM dbo.Role";

				SqlCommand cmd = new SqlCommand(sqlQuery, connection);
				var reader = cmd.ExecuteReader();

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

			return roles;
		}
	}
}
