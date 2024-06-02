namespace PortalEmpleo.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public string PostImage {  get; set; }
        public int UserId { get; set; }
    }
}
