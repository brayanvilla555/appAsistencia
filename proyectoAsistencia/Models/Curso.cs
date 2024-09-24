using proyectoAsistencia.Utils;
using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class Curso
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del curso es requerido.")]
        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Seleccionar el ciclo del curso.")]
        [Display(Name = "Ciclo")]
        public Ciclo ciclo { get; set; }

        [Required(ErrorMessage = "Seleccionar el docente del curso.")]
        [Display(Name = "Docente")]
        public int DocenteId { get; set; }
        public virtual Docente? Docentes { get; set; }
    }
}
