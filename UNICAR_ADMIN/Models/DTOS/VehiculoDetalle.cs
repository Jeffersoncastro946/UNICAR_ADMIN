namespace UNICAR_ADMIN.Models.DTOS
{
    public class VehiculoDetalle
    {
        // Datos del vehículo
        public int VehiculoId { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int Anio { get; set; }
        public string? Color { get; set; }
        public string? Vin { get; set; }
        public decimal Precio { get; set; }
        public string? Estado { get; set; }
        public int  EstadoID { get; set; }

        public string? Proveedor { get; set; }
        public int ProveedorId { get; set; }
        public bool EsConsignacion { get; set; }

        // Imágenes
        public string? FotoFrontal { get; set; }
        public string? FotoTrasera { get; set; }
        public string? FotoLateralIzquierda { get; set; }
        public string? FotoLateralDerecha { get; set; }
        public string? FotoInterior1 { get; set; }
        public string? FotoInterior2 { get; set; }
        public string? FotoMotor { get; set; }
        public string? FotoExtra1 { get; set; }
        public string? FotoExtra2 { get; set; }

        // Miniatura destacada
        public string? Miniatura { get; set; }
    }
}
