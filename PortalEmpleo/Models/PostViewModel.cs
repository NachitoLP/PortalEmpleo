using System.ComponentModel.DataAnnotations;

namespace PortalEmpleo.Models
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public byte[] PostImage { get; set; }
        public IFormFile NewPostImage { get; set; }

        public int UserId { get; set; }

        public List<Category> Categories { get; set; }
        public int CategoryId { get; set; }

        public DateTime PostDate { get; set; }
        public DateTime PostModifiedDate { get; set; }
    }
}
