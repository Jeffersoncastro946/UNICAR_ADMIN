using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Estado Contrato")]
        public string? EstadoContrato { get; set; }
        [Display(Name = "Plazo (meses)")]
        [Range(0, 480, ErrorMessage = "El plazo debe estar entre 1 y 480 meses.")]
        public int? PlazoMeses { get; set; }

        [Display(Name = "Tasa Anual (%)")]
        [Range(0, 100, ErrorMessage = "La tasa debe estar entre 0 y 100.")]
        public decimal? TasaAnual { get; set; }

        [Display(Name = "Cuota Mensual")]
        [DataType(DataType.Currency)]
        public decimal? CuotaMensual { get; set; }



        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int VendedorId { get; set; }

        [Display(Name = "Fecha Venta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime FechaVenta { get; set; }

        [Display(Name = "Precio Venta")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser positivo")]
        public decimal PrecioVenta { get; set; }

        [Display(Name = "Tipo Venta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string TipoVenta { get; set; } = "";

        // FirmaDocumento si lo vas a recoger en el UI (puedes omitirlo en DTO si no)

        public bool Activo { get; set; }

        public string? Cliente { get; set; }
        public string? Vendedor { get; set; }
        public string? vehiculo { get; set; }

        public IEnumerable<SelectListItem> ListaCliente { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ListaVendedor { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ListaVehiculo { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ListaTipoVenta { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> ListaPlazos { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ListaTasa { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ListarEstadosContrato { get; set; } = new List<SelectListItem>();

        public ListaVehiculoDTO? VehiculoDetalle { get; set; } = new ListaVehiculoDTO();

        //datos para mostrar en el index
        public decimal PrecioContrato { get; set; }
        public decimal? Pagado { get; set; }
        public DateTime? UltimoPago { get; set; }
    }
}
