using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PortalEmpleo.Models;
using System.Data.SqlClient;

namespace PortalEmpleo.Utils
{
    public static class PostUtils
    {
        public static List<Post> GetPosts()
        {
            var cs = DBHelper.GetConnectionString();

            var posts = new List<Post>();

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    string sqlQuery = @"
                        SELECT p.post_id, p.post_title, p.post_description, p.post_image, 
                               p.user_id, u.user_name, u.user_surname, u.user_profile_img, u.user_title_description,
                               p.category_id, c.category_name, p.post_date
                        FROM dbo.Posts p
                        LEFT JOIN dbo.Users u ON p.user_id = u.user_id
                        LEFT JOIN dbo.Category c ON p.category_id = c.category_id ORDER BY post_date DESC
                    ";

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Post post = new Post
                        {
                            PostId = reader.GetInt32(reader.GetOrdinal("post_id")),
                            PostTitle = reader.GetString(reader.GetOrdinal("post_title")),
                            PostDescription = reader.GetString(reader.GetOrdinal("post_description")),
                            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                            UserName = $"{reader.GetString(reader.GetOrdinal("user_name"))} {reader.GetString(reader.GetOrdinal("user_surname"))}",
                            CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
                            CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                            PostDate = reader.GetDateTime(reader.GetOrdinal("post_date"))
                        };

                        if (reader["post_image"] != DBNull.Value)
                        {
                            post.PostImage = (byte[])reader["post_image"];
                        }

                        if (reader["user_title_description"] != DBNull.Value)
                        {
                            post.UserTitle = reader.GetString(reader.GetOrdinal("user_title_description"));
                        }


                        // Verificar si user_profile_img es DBNull.Value antes de intentar acceder a él
                        if (reader["user_profile_img"] != DBNull.Value)
                        {
                            post.UserProfileImg = (byte[])reader["user_profile_img"];
                        }
                        else
                        {
                            // Si user_profile_img es DBNull.Value, asignar una imagen por defecto
                            byte[] imagenDefaultBytes = UserUtils2.ObtenerBytesImagenDefault();
                            post.UserProfileImg = imagenDefaultBytes;
                        }

                        posts.Add(post);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);

                throw new Exception("Error al obtener los Posts.", ex);
            }

            return posts;
        }

        public static List<Post> GetPostsByUser(int userId)
        {

            var cs = DBHelper.GetConnectionString();

            var posts = new List<Post>();

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();

                string sqlQuery = $"SELECT * FROM dbo.Posts WHERE user_id = {userId} ORDER BY post_date DESC";

                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Post post = new Post
                    {
                        PostId = reader.GetInt32(reader.GetOrdinal("post_id")),
                        PostTitle = (string)reader["post_title"],
                        PostDescription = (string)reader["post_description"],
                        PostDate = reader.GetDateTime(reader.GetOrdinal("post_date"))
                    };


                    if (reader["post_image"] != DBNull.Value)
                    {
                        post.PostImage = (byte[])reader["post_image"];
                    }

                    posts.Add(post);
                }

                reader.Close();
            }

            return posts;
        }

        public static List<Post> GetPostsByCategory(int categoryId) {
            var cs = DBHelper.GetConnectionString();
            var posts = new List<Post>();

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    string sqlQuery = @"
                    SELECT p.post_id, p.post_title, p.post_description, p.post_image, 
                           p.user_id, u.user_name, u.user_surname, u.user_profile_img, u.user_title_description,
                           p.category_id, c.category_name, p.post_date
                    FROM dbo.Posts p
                    LEFT JOIN dbo.Users u ON p.user_id = u.user_id
                    LEFT JOIN dbo.Category c ON p.category_id = c.category_id
                    WHERE p.category_id = @CategoryId
                    ORDER BY p.post_date DESC";

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Post post = new Post
                        {
                            PostId = reader.GetInt32(reader.GetOrdinal("post_id")),
                            PostTitle = reader.GetString(reader.GetOrdinal("post_title")),
                            PostDescription = reader.GetString(reader.GetOrdinal("post_description")),
                            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                            UserName = $"{reader.GetString(reader.GetOrdinal("user_name"))} {reader.GetString(reader.GetOrdinal("user_surname"))}",
                            CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
                            CategoryName = reader.GetString(reader.GetOrdinal("category_name")),
                            PostDate = reader.GetDateTime(reader.GetOrdinal("post_date"))
                        };

                        if (reader["post_image"] != DBNull.Value)
                        {
                            post.PostImage = (byte[])reader["post_image"];
                        }

                        if (reader["user_title_description"] != DBNull.Value)
                        {
                            post.UserTitle = reader.GetString(reader.GetOrdinal("user_title_description"));
                        }

                        if (reader["user_profile_img"] != DBNull.Value)
                        {
                            post.UserProfileImg = (byte[])reader["user_profile_img"];
                        }
                        else
                        {
                            byte[] defaultImageBytes = UserUtils2.ObtenerBytesImagenDefault();
                            post.UserProfileImg = defaultImageBytes;
                        }

                        posts.Add(post);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener los Posts por categoría.", ex);
            }

            return posts;
        }
        public static List<Category> GetCategories()
        {

            var cs = DBHelper.GetConnectionString();

            var categories = new List<Category>();

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    string sqlQuery = "SELECT * FROM Category";

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Category category = new Category
                                {
                                    CategoryId = (int)reader["category_id"],
                                    CategoryName = (string)reader["category_name"]
                                };

                                categories.Add(category);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                throw;
            }

            return categories;
        }
    }
}
