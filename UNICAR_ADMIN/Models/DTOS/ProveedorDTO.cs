using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS
{
    public class ProveedorDTO
    {
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El campo {0} es necesario")]
        public string Nombre { get; set; } = null!;

      
        public string? Telefono { get; set; }

     
        public string? Correo { get; set; }

    
        public string? Direccion { get; set; }

        public bool? Activo { get; set; }
    }
}
