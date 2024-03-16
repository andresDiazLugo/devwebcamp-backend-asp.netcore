
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace dev_backend_net.Models
{
    public class User
    {
        public long id { get; set; }
        [Required(ErrorMessage = "El nombre es un campo requerido")]
        public string name { get; set; }
        [Required(ErrorMessage = "El email es un campo requerido")]
        [EmailAddress(ErrorMessage = "El email no es un formato valido")]
        public string email { get; set; }
        [Required(ErrorMessage = "El phone es un campo requerido")]
        public string phone { get; set; }
        [Required(ErrorMessage = "El password es un campo requerido")]
        public string password { get; set; }
    }

    public class UserSignInModel
    {
        [Required(ErrorMessage = "El email es un campo requerido")]
        [EmailAddress(ErrorMessage = "El email no es un formato valido")]
        public string email { get; set; }
        [Required(ErrorMessage = "El password es un campo requerido")]
        public string password { get; set; }
    }
}