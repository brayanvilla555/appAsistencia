using proyectoAsistencia.Utils;
using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del evento es requerido.")]
        [Display(Name = "Nombre")]
        [StringLength(255)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La fecha y hora de inicio del evento es requerido.")]
        [Display(Name = "Fecha y hora de inicio")]
        public DateTime FechaHoraInicio { get; set; }

        [Required(ErrorMessage = "La fecha y hora de fin del evento es requerido.")]
        [Display(Name = "Fecha y hora de fin")]
        public DateTime FechaHoraFin { get; set; }


        [Display(Name = "Descripción del evento")]
        [StringLength(1000)]
        [MaxLength(1000)]
        public string? Descripcion { get; set; }


        [Required(ErrorMessage = "El estado del evento es requerido.")]
        public Estado Estado { get; set; }


        [Required(ErrorMessage = "La ubicación del desarrollo del evento es requerido.")]
        [Display(Name = "Ubicación")]
        [StringLength(255)]
        public string Ubicacion { get; set; }

        
        [Required]
        public int AdministradorId {  get; set; }
        public virtual Administrador? Administrador { get; set; }
        

        //Relacion con los ponentes que está relacionado
        public virtual ICollection<EventoPonente>? EventoPonentes { get; set; }


        public virtual ICollection<Asistencia>? Asistencias { get; set; }

        public Evento() {
            FechaHoraInicio = DateTime.Now;
            FechaHoraFin = DateTime.Now;
        }

        public Evento(int id, string nombre, DateTime fechaHoraInicio, DateTime fechaHoraFin, string? descripcion, Estado estado, string ubicacion)
        {
            Id = id;
            Nombre = nombre;
            FechaHoraInicio = fechaHoraInicio;
            FechaHoraFin = fechaHoraFin;
            Descripcion = descripcion;
            Estado = estado;
            Ubicacion = ubicacion;
        }
    }
}
