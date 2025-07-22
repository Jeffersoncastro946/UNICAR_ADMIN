namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class ReparacionesResponseDto
    {
        public ReparacionesKpiDto kpi { get; set; } = new ReparacionesKpiDto();
        public List<VehiculoReparacionDto> Vehiculos { get; set; } = new();

    }
}
