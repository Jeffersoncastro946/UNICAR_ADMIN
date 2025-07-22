namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class VehiculoRezagoDto
    {
        public int VehiculoId { get; set; }
        public string Vin { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public int Anio { get; set; }
        public string Color { get; set; } = "";
        public DateTime? FechaIngreso { get; set; }
        public decimal Precio { get; set; }
        public int DiasEnStock { get; set; }
        public string Rango { get; set; } = "";
    }
}
