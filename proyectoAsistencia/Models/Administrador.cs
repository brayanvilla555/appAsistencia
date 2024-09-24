using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class Administrador
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del administrador es requerido.")]
        [Display(Name = "Nombre")]
        [StringLength(70, ErrorMessage = "El nombre no puede exceder los 70 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido del administrador es requerido.")]
        [Display(Name = "Apellido")]
        [StringLength(70, ErrorMessage = "El apellido no puede exceder los 70 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El teléfono del administrador es requerido.")]
        [Display(Name = "Teléfono")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "El teléfono debe tener entre 9 y 12 caracteres.")]
        [Phone]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo electrónico válido.")]
        [StringLength(70, ErrorMessage = "El correo electrónico no puede exceder los 70 caracteres.")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "La fecha de acceso es obligatoria.")]
        [Display(Name = "Fecha de acceso")]
        public DateTime FechaAcceso { get; set; }

        // Relación con Asistencias
        public ICollection<Asistencia>? Asistencias { get; set; }

        // Relación con Eventos
        public ICollection<Evento>? Eventos { get; set; }

        // Constructor vacío
        public Administrador()
        {
            Eventos = new List<Evento>();
        }

        public Administrador(int id)
        {
            Id = id;
        }

        // Constructor con parámetros
        public Administrador(int id, string nombre, string apellido, string telefono, string email, string contrasena, DateTime fechaAcceso)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefono;
            Email = email;
            Contrasena = contrasena;
            FechaAcceso = fechaAcceso;
        }
    }
}
