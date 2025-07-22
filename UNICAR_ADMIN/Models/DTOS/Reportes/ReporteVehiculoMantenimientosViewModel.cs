namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class ReporteVehiculoMantenimientosViewModel
    {
        public int VehiculoId { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int Año { get; set; }
        public string? Color { get; set; }
        public string? VIN { get; set; }
        public decimal Precio { get; set; }
        public DateTime fechaInicial { get; set; }
        public DateTime fechaFinal { get; set; }
       
        public List<ReparacionDto>? Reparaciones { get; set; }
    }
}
