namespace PortalEmpleo.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string Description { get; set; }
        public List<User> Users { get; set; }

        public void AddUser(User user)
        {
            Users.Add(user);
        }
    }
}
