using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS
{
    public class ReparacionesDTO
    {
        
        public int ReparacionId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int VehiculoId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo.")]
        public double costo { get; set; }
        public DateTime FechaReparacion { get; set; }

        public string? ImagenUrl { get; set; }

        [Display(Name = "Foto de la reparacion")]
        public IFormFile? imageURLFormfile { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }

        //una lista de de selectitem para que selecione el vehiculo
        public IEnumerable<SelectListItem> listavehiculosVim { get; set; } = new List<SelectListItem>();


    }
}
