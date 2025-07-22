
namespace UNICAR_ADMIN.Models.DTOS
{
    public class ListaVehiculoDTO
    {
        public int VehiculoId { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int Anio { get; set; }
        public string? Color { get; set; }
        public string? Vin { get; set; }
        public decimal Precio { get; set; }
        public string? Estado { get; set; }
        public bool EsConsignacion { get; set; }
        public string? Miniatura { get; set; }
       public double? TotalGastosMantenimientos{get; set;}
    }
}
