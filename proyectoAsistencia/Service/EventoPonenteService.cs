using proyectoAsistencia.Models;

namespace proyectoAsistencia.Service
{
    public class EventoPonenteService
    {
        public List<EventoPonente> ListaEventoPonente { get; set; } = new List<EventoPonente>();
        public void Agregar(EventoPonente detalle)
        {
            ListaEventoPonente.Add(detalle);
        }
        public void Eliminar(int i)
        {
            ListaEventoPonente.RemoveAt(i);
        }
        public void EliminarTodos()
        {
            ListaEventoPonente.Clear();
        }
    }
}
