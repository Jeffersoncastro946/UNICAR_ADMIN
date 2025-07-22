using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    // Para el JSON de la gráfica
    public class IngresoMesDto
    {
        public string Mes { get; set; } = string.Empty;   // "Jun-2025"
        public decimal Monto { get; set; }   // ingresos ese mes
    }

    // Para la vista Razor
    public class ReporteFinalizadosViewModel
    {
        public decimal TotalFinalizados { get; set; }
        public List<IngresoMesDto> Meses { get; set; }= new List<IngresoMesDto>();
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
    }

}
