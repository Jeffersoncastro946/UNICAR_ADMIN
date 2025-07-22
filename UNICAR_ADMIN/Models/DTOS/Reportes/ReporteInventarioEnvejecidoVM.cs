namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class ReporteInventarioEnvejecidoVM
    {
        public int Unidades { get; set; }
        public double EdadPromedio { get; set; }
        public int EdadMaxima { get; set; }
        public decimal ValorTotal { get; set; }
        public double? PorcInventarioTotal { get; set; }  // opcional
        public List<DistribucionRangoDto> Rangos { get; set; } = new();
        public List<VehiculoRezagoDto> Vehiculos { get; set; } = new();
    }
}
