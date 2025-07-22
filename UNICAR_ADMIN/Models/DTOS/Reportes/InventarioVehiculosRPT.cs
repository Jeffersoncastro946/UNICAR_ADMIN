using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class InventarioVehiculosRPT
    {
        public int CodVehiculo { get; set; }
        public string Marca { get; set; }=string.Empty;
        public string Modelo {  get; set; }=string.Empty;
        public DateTime? FechaIngreso { get; set; }
        public string Estado { get; set; }=string.Empty ;
        public int Anio { get; set; }
        public string color { get; set; }= string.Empty ;
        public string Vin { get; set; }  = string.Empty ;   
        
        public string Proveedor { get; set; } =string.Empty ;  
        public decimal PrecioAdquisicion { get; set; }  
        public bool Esconsignacion { get; set; }

    }

}
