using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS
{
    public class ContratoDto
    {
        public int ContratoId { get; set; }

        [Required]
        public int VehiculoId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int VendedorId { get; set; }

        [Display(Name = "Fecha Venta")]
        [Required]
        public DateTime FechaVenta { get; set; }

        [Display(Name = "Precio Venta")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser positivo")]
        public decimal PrecioVenta { get; set; }

        [Display(Name = "Tipo Venta")]
        [Required]
        public string TipoVenta { get; set; } = "";

        // FirmaDocumento si lo vas a recoger en el UI (puedes omitirlo en DTO si no)
        public string? FirmaDocumento { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Firma Documento")]
        IFormFile? FirmaDocumentoFile { get; set; }
        public bool Activo { get; set; } 

        public string? Cliente { get; set; }
        public string? Vendedor { get; set; }


    }
}
