namespace PortalEmpleo.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public byte[] PostImage {  get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserTitle { get; set; }
        public byte[] UserProfileImg { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime PostDate { get; set; }
    }
}
