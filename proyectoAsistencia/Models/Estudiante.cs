using proyectoAsistencia.Utils;
using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class Estudiante
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código del estudiante es requerido.")]
        [Display(Name = "Código de estudiante")]
        [StringLength(10)]
        public string CodigoEstudiante { get; set; }

        [Required(ErrorMessage = "El Dni del estudiante es requerido.")]
        [Display(Name = "Dni")]
        [StringLength(8)]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El nombre del estudiante es requerido.")]
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido del estudiante es requerido.")]
        [Display(Name = "Apellido")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La promoción del estudiante es requerida.")]
        [Display(Name = "Promoción")]
        [StringLength(6)]
        public string Promocion { get; set; }

        [Required(ErrorMessage = "El teléfono del estudiante es requerido.")]
        [Display(Name = "Teléfono")]
        [StringLength(12, MinimumLength =9)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El siclo del estudiante es requerido.")]
        [Display(Name = "Ciclo")]
        public Ciclo Ciclo { get; set; }

        [Required(ErrorMessage = "El email del estudiante es requerido.")]
        [Display(Name = "Email")]
        [StringLength(70)]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string? Contrasena { get; set; }

        //Relacion
        public virtual ICollection<Asistencia>? Asistencias { get; set; }

        public Estudiante()
        {

        }

        public Estudiante(int id, string codigoEstudiante, string nombre, string apellido, string promocion, string telefono, string email, string contrasena)
        {
            Id = id;
            CodigoEstudiante = codigoEstudiante;
            Nombre = nombre;
            Apellido = apellido;
            Promocion = promocion;
            Telefono = telefono;
            Email = email;
            Contrasena = contrasena;
        }
    }
}
