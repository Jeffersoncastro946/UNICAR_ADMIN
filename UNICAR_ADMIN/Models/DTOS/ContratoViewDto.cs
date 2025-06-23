using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS
{
    public class ContratoViewDto 
    {
        public int ContratoId { get; set; }

        // En lugar de VehiculoId, mostramos algo descriptivo:
        public string Vehiculo { get; set; } = string.Empty;   // p.ej. "Toyota Corolla 2020"


        // En lugar de VendedorId, el nombre del vendedor:
        public string Vendedor { get; set; } = string.Empty;   // p.ej. "María López"

        public DateTime FechaVenta { get; set; }
        [Display(Name = "Días desde la venta")]
        public int DiasDesdeVenta { get; set; }

        public decimal PrecioVenta { get; set; }

        public string TipoVenta { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}
