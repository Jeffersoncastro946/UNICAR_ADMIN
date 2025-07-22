namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class VehiculoReparacionDto
    {
        public string VIN { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public decimal PrecioVenta {get; set;}
        public decimal TotalReparaciones { get; set; }
 
        public string Estado { get; set; } = string.Empty;
        public string Observacion { get; set; }= string.Empty;

    }
}
