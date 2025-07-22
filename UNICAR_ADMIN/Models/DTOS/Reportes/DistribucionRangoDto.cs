namespace UNICAR_ADMIN.Models.DTOS.Reportes
{
    public class DistribucionRangoDto
    {
        public string Rango { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Valor { get; set; }
        public double Porcentaje { get; set; } // 0–1
    }

}
