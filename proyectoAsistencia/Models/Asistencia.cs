using System;
using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Verifica que estés seleccionando el estudiante correcto")]
        public int EstudianteId { get; set; }
        public virtual Estudiante? Estudiante { get; set; }

        [Required(ErrorMessage = "Verifica que estés seleccionando el evento correcto")]
        [Display(Name = "Selecciona el evento")]
        public int EventoId { get; set; }
        public virtual Evento? Evento { get; set; }

        [Required(ErrorMessage = "Verifica que estés seleccionando el administrador correcto")]
        public int AdministradorId { get; set; }
        public Administrador? Administrador { get; set; }

        [Required]
        public DateTime FechaHoraRegistro { get; set; }

        // Constructor sin parámetros, inicializa la fecha de registro
        public Asistencia()
        {
            FechaHoraRegistro = DateTime.Now;
        }

        // Constructor con parámetros
        public Asistencia(int id, int estudianteId, Estudiante estudiante, int eventoId, Evento evento, int administradorId, Administrador administrador, DateTime fechaHoraRegistro)
        {
            Id = id;
            EstudianteId = estudianteId;
            Estudiante = estudiante;
            EventoId = eventoId;
            Evento = evento;
            AdministradorId = administradorId;
            Administrador = administrador;
            FechaHoraRegistro = fechaHoraRegistro;
        }
    }
}
