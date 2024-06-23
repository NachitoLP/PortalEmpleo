using Microsoft.AspNetCore.Mvc;
using PortalEmpleo.Models;
using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using PortalEmpleo.Utils;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PortalEmpleo.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HomeView homeview = new HomeView
            {
                
            };

            List<Post> userPosts = PostUtils.GetPosts();
            List<Category> categories = PostUtils.GetCategories();

            homeview.Posts = userPosts;
            homeview.Categories = categories;
            return View(homeview);
        }

        public IActionResult GetPostsByCategory(int categoryId)
        {
            var posts = PostUtils.GetPostsByCategory(categoryId);

            return PartialView("_PostsPartialView", posts);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost(PostViewModel model)
        {
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Correo")?.Value;

            var user = UserUtils2.getUser(userEmail);

            SqlConnectionStringBuilder connectionString = new();

            var cs = DBHelper.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                if (model.NewPostImage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.NewPostImage.CopyToAsync(memoryStream);
                        model.PostImage = memoryStream.ToArray();
                    }
                    SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Posts(post_title, post_description, user_id, category_id, post_image, post_date) VALUES(@PostTitle, @PostDescription, @UserId, @CategoryId, @PostImage, @PostDate)", connection);
                    cmd.Parameters.AddWithValue("@PostTitle", model.PostTitle);
                    cmd.Parameters.AddWithValue("@PostDescription", model.PostDescription);
                    cmd.Parameters.AddWithValue("@UserId", user.UserId);
                    cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                    cmd.Parameters.AddWithValue("@PostImage", model.PostImage);
                    cmd.Parameters.AddWithValue("@PostDate", DateTime.Now);

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Posts(post_title, post_description, user_id, category_id, post_date) VALUES(@PostTitle, @PostDescription, @UserId, @CategoryId, @PostDate)", connection);
                    cmd.Parameters.AddWithValue("@PostTitle", model.PostTitle);
                    cmd.Parameters.AddWithValue("@PostDescription", model.PostDescription);
                    cmd.Parameters.AddWithValue("@UserId", user.UserId);
                    cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                    cmd.Parameters.AddWithValue("@PostDate", DateTime.Now);

                    cmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
