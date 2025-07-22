namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class FiltroContratosEstadoDto
    {
        public required string Estado { get; set; }          // "Activo", "Cancelado", "EnMora", "Finalizado", "Todos"
        public DateTime? FechaInicio { get; set; }  // null = sin límite
        public DateTime? FechaFin { get; set; }     // null = hoy
    }
}
