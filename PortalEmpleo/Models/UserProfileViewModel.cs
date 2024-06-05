namespace PortalEmpleo.Models
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public DateTime UserBirthDate { get; set; }
        public string UserProfileImg { get; set; }
        public string RoleDescription { get; set; }

        // Otras propiedades necesarias para la vista de perfil
    }
}
