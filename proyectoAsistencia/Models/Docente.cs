using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class Docente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del docente es requerido.")]
        [Display(Name = "Nombre")]
        [StringLength(70, ErrorMessage = "El nombre no puede exceder los 70 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido del docente es requerido.")]
        [Display(Name = "Apellido")]
        [StringLength(70, ErrorMessage = "El apellido no puede exceder los 70 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El teléfono del docente es requerido.")]
        [Display(Name = "Teléfono")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "El teléfono debe tener entre 9 y 12 caracteres.")]
        [Phone(ErrorMessage = "Debes ingresar un número de teléfono válido.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo electrónico válido.")]
        [StringLength(70, ErrorMessage = "El correo electrónico no puede exceder los 70 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener un máximo de 100 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "La fecha de acceso es obligatoria.")]
        [Display(Name = "Fecha de acceso")]
        public DateTime FechaAcceso { get; set; }

        //relacion con el curso
        public virtual ICollection<Curso>? Cursos { get; set; }

    }
}
