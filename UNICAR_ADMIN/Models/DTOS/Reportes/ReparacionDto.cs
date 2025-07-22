namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class ReparacionDto
    {
        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; }
        public decimal Costo { get; set; }
        public string? ImagenUrl { get; set; }
        public string? Responsable { get; set; }
    }
}
