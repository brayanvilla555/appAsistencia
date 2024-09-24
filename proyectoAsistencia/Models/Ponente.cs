using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class Ponente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del ponente es requerido.")]
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido del ponente es requerido.")]
        [Display(Name = "Apellido")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La especialización del ponente es requerido.")]
        [Display(Name = "Especialización")]
        [StringLength(255)]
        public string Especializacion { get; set; }

        [Required(ErrorMessage = "El email del ponente es requerido.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo electrónico válido.")]
        [StringLength(70)]
        public string Email { get; set; }


        [Required(ErrorMessage = "El teléfono del ponente es requerido.")]
        [Display(Name = "Teléfono")]
        [StringLength(15, MinimumLength = 9)]
        [Phone]
        public string Telefono { get; set; }

        public virtual ICollection<EventoPonente>? EventoPonentes { get; set; }


        public Ponente()
        {

        }

        public Ponente(int id, string nombre, string apellido, string especializacion, string email, string telefono)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Especializacion = especializacion;
            Email = email;
            Telefono = telefono;
        }
    }
}
