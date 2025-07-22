namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class ReporteHeaderViewModel
    {
        public string Titulo { get; set; }=string.Empty;    
        public string Subtitulo { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
    }
}
