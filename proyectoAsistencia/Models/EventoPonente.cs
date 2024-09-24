using System.ComponentModel.DataAnnotations;

namespace proyectoAsistencia.Models
{
    public class EventoPonente
    {
        //sin id
        [Required(ErrorMessage = "Selecciona el vento")]
        public int EventoId { get; set; }
        public virtual Evento? Evento { get; set; }

        [Required(ErrorMessage = "Selecciona el o los ponentes")]
        public int PonenteId { get; set; }
        public virtual Ponente? Ponente { get; set; }
    }
}
