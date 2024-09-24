using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public string Role { get; set; }
    }
}
