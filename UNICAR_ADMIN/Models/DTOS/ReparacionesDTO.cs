using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS
{
 
        /// <summary>
        /// DTO único para Detalle, Crear y Editar Reparaciones
        /// </summary>
        public class ReparacionesDTO
        {
            /* ──────────── Identificadores ──────────── */
            public int ReparacionId { get; set; }

            [Required(ErrorMessage = "Debe elegir un vehículo.")]
            [Display(Name = "Vehículo (VIN)")]
            public int VehiculoId { get; set; }

        /* ──────────── Reparación ──────────── */
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo.")]
        [Display(Name = "Costo (L)")]
        public double? Costo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de inicio no es válida.")]
        [Display(Name = "Fecha inicio")]
        public DateTime? FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de fin no es válida.")]
        [Display(Name = "Fecha fin")]
        public DateTime? FechaFin { get; set; }

        [Required(ErrorMessage = "El responsable es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre del responsable no puede superar los 200 caracteres.")]
        public string? Responsable { get; set; }


        /* Foto */

        public string? ImagenUrl { get; set; }

        [Display(Name = "Foto de la reparación")]
        public IFormFile? ImagenFile { get; set; }


        /* ──────────── Datos del vehículo (solo lectura en Detalle) ──────────── */
        [Display(Name = "VIN")]
            public string Vin { get; set; } = string.Empty;
            public string Marca { get; set; } = string.Empty;
            public string Modelo { get; set; } = string.Empty;
            public int Anio { get; set; }
            public string Color { get; set; } = string.Empty;

            /* ──────────── Dropdown para el formulario ──────────── */
            public IEnumerable<SelectListItem> ListaVehiculosVin { get; set; }
            = new List<SelectListItem>();

        }
}
