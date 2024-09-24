using proyectoAsistencia.Models;

namespace proyectoAsistencia.ViewModel
{
    public class CreateEventoViewModel
    {
        public Evento? Evento { get; set; }
        public List<int>? SelectedPonentesIds { get; set; }
        public List<Ponente>? Ponentes { get; set; }
    }
}
