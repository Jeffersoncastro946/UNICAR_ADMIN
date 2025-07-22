namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class DashboardClientesVM
    {
        public int ClientesActivos { get; set; }
        public int NuevosMesActual { get; set; }
        public int NuevosAnio { get; set; }
        public double CrecimientoMesActualPct { get; set; }
        public double TasaCancelacionPct { get; set; }
        public List<ClienteCrecimientoDto> Series { get; set; } = new();

    }
}
