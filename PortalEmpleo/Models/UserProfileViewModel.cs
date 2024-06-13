using System.ComponentModel.DataAnnotations;

namespace PortalEmpleo.Models
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public DateTime UserBirthDate { get; set; }
        public byte[] UserProfileImg { get; set; }
        public string RoleDescription { get; set; }

        //[Display(Name = "Seleccionar nueva imagen de perfil")]
        public IFormFile NewProfileImage { get; set; }
        public List<Role> Roles { get; set; }
        public string SelectedRole { get; set; }
        public string FutureDateError { get; set; }
    }
}
