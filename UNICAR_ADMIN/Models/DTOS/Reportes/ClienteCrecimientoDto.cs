using System.Globalization;

namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class ClienteCrecimientoDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Nuevos { get; set; }
        public int Acumulado { get; set; }
        public double? CrecimientoPct { get; set; } // vs mes anterior
        public string Etiqueta { get; set; }="";
    }

}
